using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunctionCollapse
{
    public class Position
    {
        private int length;
        private string[] images;
        private string actualPosition;
        private string imageName;

        public Position(int length,string actualPosition,string imageName)
        {
            this.length = length;
            this.ActualPosition = actualPosition;
            this.ImageName = imageName;
            images = new string[length];

            for (int i = 0; i < length; i++)
            {
                images[i] = new string(String.Empty);
            }
        }

        public int Length { get => length; set => length = value; }
        public string[] Images { get => images; set => images = value; }
        public string ActualPosition { get => actualPosition; set => actualPosition = value; }
        public string ImageName { get => imageName; set => imageName = value; }
    }

}
