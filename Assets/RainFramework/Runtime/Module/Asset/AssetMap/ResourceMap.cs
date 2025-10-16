// code generation.

using System.Collections.Generic;

namespace Rain.Core
{
   public static class ResourceMap
   {
       public static Dictionary<string, string[]> Mappings
       {
           get => mappings;
           set => mappings = value;
       }
       
       private static Dictionary<string, string[]> mappings = new Dictionary<string, string[]> {
       };
   }
}