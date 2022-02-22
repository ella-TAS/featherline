using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Featherline
{
    public class NumMenuItem
    {
        private ToolStripMenuItem inner;
        private string varName;

        private decimal val;
        public decimal Value
        {
            get => val;
            set {
                val = Math.Max(min, Math.Min(max, decimal.Round(value, decPlaces)));
                inner.Text = $"{varName}: {val}";
            }
        }

        private decimal min;
        private decimal max;
        private int decPlaces;

        public void PromptNewValue() {
            Form prompt = new Form() {
                Width = 305,
                Height = 102,
                Text = $"Select a value for {varName}",
                ShowIcon = false,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false,
                MaximizeBox = false,
            };
            NumericUpDown input = new NumericUpDown() {
                Minimum = min, Maximum = max, Value = Value, DecimalPlaces = decPlaces,
                Left = 20,
                Top = 20,
                Width = 150,
            };
            Button btn = new Button() {
                Text = "Ok",
                Left = 200,
                Width = 70,
                Top = 19,
                Height = 25,
            };

            btn.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(btn);
            prompt.Controls.Add(input);
            prompt.ShowDialog();

            Value = onValueUpdate is null ? input.Value : onValueUpdate(input.Value);
        }

        public Func<decimal, decimal> onValueUpdate;

        public NumMenuItem(ToolStripMenuItem inner, decimal min, decimal max, int places, Func<(decimal, decimal)> getMinMax = null)
        {
            this.inner = inner;
            varName = inner.Text;
            this.min = decimal.Round(min, places);
            this.max = decimal.Round(max, places);
            decPlaces = places;
            inner.Click += (s, e) => {
                if (getMinMax != null)
                    (min, max) = getMinMax();
                PromptNewValue();
            };
        }
    }
}
