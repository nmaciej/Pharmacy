using System;
using System.Collections.Generic;
using System.Text;

namespace Main
{
  class Program
  {
    static void Main(string[] args)
    {
      Menu();
    }

    public static void Menu()
    {
      var loopControl = true;

      while (loopControl)
      {
        Console.Clear();
        UserInterface.MenuUi();

        var input = Console.ReadLine();
        switch (input)
        {
          case "Add":
          case "a":
          case "1":
            UserInterface.MedicineAdd();
            break;

          case "EditMedicine":
          case "e":
          case "2":
            UserInterface.MedicineEdit();
            break;

          case "RemoveMedicine":
          case "r":
          case "3":
            UserInterface.MedicineRemove();
            Console.ReadKey();
            break;

          case "ListMedicines":
          case "l":
          case "4":
            UserInterface.MedicinesList();
            Console.ReadKey();
            break;

          case "AddOrder":
          case "ao":
          case "5":
            UserInterface.OrderAdd();
            break;

          case "ListOrders":
          case "lo":
          case "6":
            UserInterface.OrdersList();
            break;

          case "Exit":
          case "ex":
          case "7":
            loopControl = UserInterface.ProgramExit();
            break;
        }
      }
    }
  }
}
