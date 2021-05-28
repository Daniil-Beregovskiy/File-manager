using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace File_manager
{
    class ListViewColumnComparer : IComparer
    {
        public int ColumnIndex { get; set; }
        private SortOrder SortOrder { get; set; }
        public ListViewColumnComparer(int columnIndex, SortOrder sort_order)
        {
            ColumnIndex = columnIndex;
            SortOrder = sort_order;
        }

        public int Compare(object x, object y)
        {
            try
            {
                if (SortOrder == SortOrder.Ascending)
                {
                    return String.Compare(
                ((ListViewItem)x).SubItems[ColumnIndex].Text,
                ((ListViewItem)y).SubItems[ColumnIndex].Text);
                }
                else
                {
                    return -String.Compare(
                ((ListViewItem)x).SubItems[ColumnIndex].Text,
                ((ListViewItem)y).SubItems[ColumnIndex].Text);
                }

            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
