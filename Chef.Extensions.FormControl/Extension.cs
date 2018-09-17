using System.Windows.Forms;

namespace Chef.Extensions.FormControl
{
    public static class Extension
    {
        public static void InvokeIfNecessary(this Control control, MethodInvoker action)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(action);
            }
            else
            {
                action();
            }
        }
    }
}