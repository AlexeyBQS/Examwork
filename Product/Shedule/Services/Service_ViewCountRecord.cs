using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Shedule.Services
{
    public static partial class Service
    {
        public class ViewCountRecord
        {
            public ViewCountRecord(string name, int count)
            {
                Name = name;
                Count = count;
            }

            public string Name { get; set; } = null!;
            public int Count { get; set; } = default;

            public override string ToString() => Name;
        }

        public static void FillViewCountRecordComboBox(ComboBox sender)
        {
            sender.ItemsSource = new ViewCountRecord[]
            {
                new ViewCountRecord("50 строк", 50),
                new ViewCountRecord("100 строк", 100),
                new ViewCountRecord("1000 строк", 1000),
                new ViewCountRecord("Все", -1)
            };

            sender.SelectedIndex = 2;
        }
    }
}
