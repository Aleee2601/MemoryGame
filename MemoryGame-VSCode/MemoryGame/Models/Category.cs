using System.Collections.Generic;
using System.IO;

namespace MemoryGame.Models
{
    public class Category
    {
        public string Name { get; set; }
        public string FolderPath { get; set; }
        public List<string> ImagePaths { get; set; } = new List<string>();

        public Category()
        {
        }

        public Category(string name, string folderPath)
        {
            Name = name;
            FolderPath = folderPath;
            LoadImages();
        }

        private void LoadImages()
        {
            if (Directory.Exists(FolderPath))
            {
                string[] files = Directory.GetFiles(FolderPath, "*.jpg");
                foreach (string file in files)
                {
                    ImagePaths.Add(file);
                }

                files = Directory.GetFiles(FolderPath, "*.png");
                foreach (string file in files)
                {
                    ImagePaths.Add(file);
                }

                files = Directory.GetFiles(FolderPath, "*.gif");
                foreach (string file in files)
                {
                    ImagePaths.Add(file);
                }
            }
        }
    }
}
