using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BricksStyle.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BricksStyle.VariableTemplate
{
    public class VariableTiles : DataTemplateSelector
    {
       
        public DataTemplate BigTemplate { get; set; }
        public DataTemplate SmallTemplate { get; set; }
        
    
      

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element!=null && item != null)
            {
                
                if ((item as SampleDataItem).UniqueId.StartsWith("Big"))
                    return BigTemplate; 
                if ((item as SampleDataItem).UniqueId.StartsWith("Small"))
                    return SmallTemplate;
               
                
            }
            return base.SelectTemplateCore(item, container);
        }

    }
}
