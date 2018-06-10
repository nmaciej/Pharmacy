using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Main
{
  class UserInterface
  {
    public static void MedicineAdd()
    {

      var newMedicine = MedicineCreationWizard();
      newMedicine.Add();

      Console.Clear();
      OutInColor("New medicine",ConsoleColor.DarkGreen);
      TopBarMedicine();
      Console.WriteLine(newMedicine.ShowMedicine(Medicine.ShowMedicines().Count));
      Console.ReadLine();
    }

    public static void MedicineRemove()
    {
      MedicinesList();
      var medicineList = Medicine.ShowMedicines();

      if (medicineList is null || medicineList.Count == 0)
      {
        Console.WriteLine("Medicines database is Empty. Navigating to Main Menu");
        Program.Menu();
      }
      else
      {
        var inputId = UserInterface.AskForMedicineId(medicineList, "Deleted");
        medicineList[inputId - 1].RemoveMedicine();
      }
    }

    public static void MedicineEdit()
    {
      MedicinesList();
      var medicineList = Medicine.ShowMedicines();

      if (medicineList is null || medicineList.Count == 0)
      {
        Console.WriteLine("Medicines database is Empty. Navigating to Main Menu");
        Program.Menu();
      }


      var inputId = UserInterface.AskForMedicineId(medicineList, "Edited");

      var toBeEdited = medicineList[inputId - 1];

      TopBarMedicine();
      Console.WriteLine(toBeEdited.ShowMedicine(inputId));
      Console.ReadLine();

      var edited = MedicineCreationWizard();

      //toBeEdited is object from database and have an actual id from database
      edited.Id = toBeEdited.Id;

      OutInColor("Old record:", ConsoleColor.DarkRed);
      TopBarMedicine();
      OutInColor(toBeEdited.ShowMedicine(inputId));
      OutInColor("New record:", ConsoleColor.DarkGreen);
      TopBarMedicine();
      OutInColor(edited.ShowMedicine(inputId));

      edited.EditMedicine();

      Console.ReadLine();
    }

    public static List<Medicine> MedicinesList()
    {
      var listOfMedicines = Medicine.ShowMedicines();

      if (listOfMedicines.Count >= 1)
      {
        Console.WriteLine("List of Medicines:\n");

        Console.Clear();
        TopBarMedicine();

        int i = 1;
        foreach (var med in listOfMedicines)
        {
          Console.WriteLine(med.ShowMedicine(i));
          i++;
        }
        Console.ReadLine();
      }
      else
      {
        Console.WriteLine("There are no medicines in the database");
        Thread.Sleep(1000);
      }

      return listOfMedicines;
    }

    public static void OrdersList()
    {
      var listOfOrders = Order.ShowOrders();

      if (listOfOrders.Count >= 1)
      {
        Console.Clear();
        TopBarOrder();
        int i = 1;
        foreach (var order in listOfOrders)
        {
          Console.WriteLine(order.ShowOrder(i));
          i++;
        }

        Console.WriteLine("\n");
        Thread.Sleep(1000);
        Console.ReadKey();
      }
      else
      {
        Console.WriteLine("There are no Orders in the database");
        Thread.Sleep(1000);
      }
    }

    public static void OrderAdd()
    {
      //Extracting list of available medicines
      var medicineList = new List<Medicine>();

      //Shoppin Cart - will keep list of medicines customer wants to buy
      var shoppingCart = new List<CartItem>();

      //Loop to choose which medicine will go to cart
      while (true)
      {
        //Asking for id of the medicine from printed list
        medicineList = MedicinesList();
        var id = AskForMedicineId(medicineList, "added to order");
        Console.ReadKey();

        //Medicine can noot be already in the cart
        if (shoppingCart.Any(x => x.Id == id))
        {
          Console.WriteLine("Item is already in your cart!");
        }
        else
        {
          var amount = AmountCheck(medicineList[id - 1].Amount);

          //If medicine is selled using prescription it's number needs to be archived
          if (medicineList[id - 1].Prescription)
          {
            var prescription = PrescriptionNumber();

            shoppingCart.Add(new CartItem() { Id = id, Amount = amount, Prescription = prescription });
          }
          else
          {
            shoppingCart.Add(new CartItem() { Id = id, Amount = amount, Prescription = null });
          }
        }

        //After adding medicine - choose next medicine or save oredr to database
        var command = AddNextMedicine();
        if (command == "NO")
        {
          break;
        }
      }

      //Calculating order cost
      decimal orderCost = 0;
      foreach (var item in shoppingCart)
      {
        orderCost += item.Amount * medicineList[item.Id - 1].Price;
      }

      //Adding new Order to database
      var newOrder = new Order(DateTime.Now, orderCost);
      var newOrderId = newOrder.Add();

      //Removing selled medicines from the stock
      foreach (var item in shoppingCart)
      {
        var updateAmount = medicineList[item.Id - 1];

        updateAmount.Amount -= item.Amount;
        updateAmount.EditMedicine();
      }

      //Creating entry in the MedicinesOrders database
      foreach (var item in shoppingCart)
      {
        var record = new MedicinesOrders(newOrderId, medicineList[item.Id - 1].Id, item.Amount, item.Prescription);
        record.Add();
      }

      OutInColor("The following order was added to database:", ConsoleColor.DarkGreen);
      Console.WriteLine("Name, Amount, Cost");

      foreach (var item in shoppingCart)
      {
        Console.WriteLine($"{medicineList[item.Id - 1].Name}, {item.Amount}, {item.Amount * medicineList[item.Id - 1].Price}");
      }
      Console.WriteLine($"Total price is: {orderCost}");

      Console.ReadLine();
    }

    public static bool ProgramExit()
    {
      Console.Clear();
      OutInColor("Closing program...", ConsoleColor.Yellow, ConsoleColor.Red);
      Thread.Sleep(2000);
      return false;
    }

    //SUPPORTING FUNCTIONS
    public static int AskForMedicineId(List<Medicine> listMedicines, string text)
    {
      while (true)
      {
        Console.WriteLine($"Enter Id of medicine to be {text} or 0 to exit.");
        var inputId = Int32.TryParse(Console.ReadLine(), out var parsedId);

        //check of parsed successfully
        if (inputId)
        {
          //if user wants to go to main menu
          if (parsedId == 0)
          {
            Program.Menu();
          }
          //check if id is in range
          else if (parsedId < 1 || parsedId > listMedicines.Count)
          {
            OutInColor("Id is out of range!", ConsoleColor.DarkRed);
            Thread.Sleep(1000);
          }
          else
          {
            //if id is a number and within range return it as valid id
            return parsedId;
          }
        }
        //if parsed unsuccessfully
        else
        {
          OutInColor("Id is a number!", ConsoleColor.DarkYellow);
          Thread.Sleep(1000);
        }
      }
    }

    private static string ManufacturerNameCheck(string field, int length)
    {
      while (true)
      {
        Console.Clear();
        Console.WriteLine($"Enter medicine {field}:");
        var text = Console.ReadLine();

        if (text.Length >= 3 && text.Length <= length)
        {
          return text;
        }
        else if (text.ToLower() == "exit")
        {
          Program.Menu();
        }
        else
        {
          OutInColor("Input string shall be between 3 and 40 chars.", ConsoleColor.DarkYellow);
          Thread.Sleep(1000);
        }
      }
    }

    private static decimal PriceCheck()
    {
      while (true)
      {
        Console.Clear();
        Console.WriteLine("Enter medicine Price:");
        var price = Decimal.TryParse(Console.ReadLine(), out var parsedPrice);

        if (price)
        {
          if (parsedPrice > 0 && parsedPrice < 100)
          {
            return parsedPrice;
          }
          else
          {
            OutInColor("Price shall be between 0 and 100.", ConsoleColor.DarkYellow);
            Thread.Sleep(1000);
          }
        }
        else
        {
          OutInColor("Price is a number!", ConsoleColor.DarkRed);
          Thread.Sleep(1000);
        }
      }
    }

    private static int AmountCheck(int maxAmount)
    {
      while (true)
      {
        Console.Clear();
        Console.WriteLine("Enter medicine Amount:");
        var price = Int32.TryParse(Console.ReadLine(), out var parsedAmount);

        if (price)
        {
          if (parsedAmount > 0 && parsedAmount <= maxAmount)
          {
            return parsedAmount;
          }
          else
          {
            OutInColor($"Amount shall be between 0 and {maxAmount}.", ConsoleColor.DarkYellow);
            Thread.Sleep(1000);
          }
        }
        else
        {
          OutInColor("Amount is a number!", ConsoleColor.DarkRed);
          Thread.Sleep(1000);
        }
      }
    }

    private static bool PrescriptionCheck()
    {
      while (true)
      {
        Console.Clear();
        Console.WriteLine("Does this medicine needs prescription? (Yes/No)");
        string prescription = Console.ReadLine();

        if (prescription != null && prescription.ToLower() == "yes")
        {
          return true;
        }
        else if (prescription != null && prescription.ToLower() == "no")
        {
          return false;
        }
        else
        {
          OutInColor("Enter Yes or No.", ConsoleColor.DarkRed);
          Thread.Sleep(1000);
        }
      }
    }

    private static string PrescriptionNumber()
    {
      while (true)
      {
        Console.WriteLine("This medicine need prescription. Enter prescription number.");
        string prescription = Console.ReadLine();

        if (prescription != null && prescription.Length == 7 && prescription.All(Char.IsDigit))
        {
          return prescription;
        }
        else
        {
          OutInColor("Prescription is a 7 digit number.", ConsoleColor.DarkRed);
          Thread.Sleep(1000);
        }
      }
    }

    private static Medicine MedicineCreationWizard()
    {
      var manufacturer = ManufacturerNameCheck("Manufacturer", 40);
      var name = ManufacturerNameCheck("Name", 40);
      var price = PriceCheck();
      var amount = AmountCheck(100);
      var prescription = PrescriptionCheck();

      return new Medicine(manufacturer, name, price, amount, prescription);
    }

    private static string AddNextMedicine()
    {
      while (true)
      {

        Console.WriteLine("Do you want to add next item? (Yes/No) or Exit for Main Menu");
        var command = Console.ReadLine();

        if (command.ToLower() == "yes")
        {
          return "YES";
        }
        else if (command.ToLower() == "no")
        {
          return "NO";
        }
        else if (command.ToLower() == "exit")
        {
          Program.Menu();
        }
        else
        {
          OutInColor("Enter valid option (Yes/No)", ConsoleColor.DarkRed);
          Console.ReadLine();
          AddNextMedicine();
        }
      }
    }

    public static void TopBarMedicine()
    {
      StringBuilder table = new StringBuilder();

      table.Append("".PadLeft(73, '-'));
      table.Append("\n");
      table.Append("|" + $"ID".PadLeft(3) + "".PadLeft(1));
      table.Append("|" + $"Medicine".PadLeft(15) + "".PadLeft(1));
      table.Append("|" + $"Company".PadLeft(15) + "".PadLeft(1));
      table.Append("|" + $"Price".PadLeft(7) + "".PadLeft(1));
      table.Append("|" + $"Amount".PadLeft(7) + "".PadLeft(1));
      table.Append("|" + $"Prescription".PadLeft(13) + "".PadLeft(1) + "|");
      table.Append("\n");
      table.Append("".PadLeft(73, '-'));

      Console.WriteLine(table.ToString());
    }

    public static void TopBarOrder()
    {
      StringBuilder table = new StringBuilder();

      table.Append("".PadLeft(40, '-'));
      table.Append("\n");
      table.Append("|" + $"ID".PadLeft(3) + "".PadLeft(1));
      table.Append("|" + $"Date".PadLeft(15) + "".PadLeft(1));
      table.Append("|" + $"Cost".PadLeft(15) + "".PadLeft(1)+"|");
      table.Append("\n");
      table.Append("".PadLeft(40, '-'));

      Console.WriteLine(table.ToString());
    }

    public static void MenuUi()
    {
      var table = new StringBuilder();

      table.Append("".PadLeft(28,'-'));
      table.Append("\n");
      table.Append("|"+$"No".PadLeft(3)+"".PadLeft(1));
      table.Append("|"+$"Option".PadLeft(20)+"".PadLeft(1)+"|");
      table.Append("\n");
      table.Append("".PadLeft(28,'-'));

      var menuList = new List<string>()
      {
        "Add Medicine (a)",
        "Edit Medicine (e)",
        "Remove Medicine (r)",
        "List Medicines (l)",
        "Add Order (ao)",
        "List Orders (lo)",
        "Exit ex"
      };

      int i = 1;
      foreach (var line in menuList)
      {
        table.Append("\n");
        table.Append("|"+$"{i}".PadLeft(3)+"".PadLeft(1));
        table.Append("|"+$"{line}".PadLeft(20)+"".PadLeft(1)+"|");
        table.Append("\n");
        table.Append("".PadLeft(28,'-'));

        i++;
      }
      Console.WriteLine(table.ToString());
    }

    public static void OutInColor(string text, ConsoleColor bColor = ConsoleColor.Black, ConsoleColor fColor = ConsoleColor.White)
    {

      Console.BackgroundColor = bColor;
      Console.ForegroundColor = fColor;
      Console.WriteLine(text);
      Console.ResetColor();
    }
  }
}
