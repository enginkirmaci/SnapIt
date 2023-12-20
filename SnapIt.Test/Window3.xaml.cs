//using System.Windows;
//using System.Windows.Automation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using Gma.System.MouseKeyHook;
using SnapIt.Common;
using SnapIt.Common.Entities;
using SnapIt.Services;

namespace SnapIt.Test;

public enum DragElement
{
    None,
    Window,
    Tab
}

public partial class Window3 : Window
{
    private IKeyboardMouseEvents globalHook;
    private WinApiService winApiService;
    private AutomationElementCollection firefoxes;
    private AutomationElementCollection currentfirefoxes;
    private ActiveWindow active;
    private bool isListening = false;
    private bool isDragging = false;
    private DragElement element = DragElement.None;

    public Window3()
    {
        InitializeComponent();

        globalHook = Hook.GlobalEvents();

        globalHook.MouseMove += GlobalHook_MouseMove;
        globalHook.MouseDown += GlobalHook_MouseDown;
        globalHook.MouseUp += GlobalHook_MouseUp;

        globalHook.MouseDragStarted += GlobalHook_MouseDragStarted;
        globalHook.MouseDragFinished += GlobalHook_MouseDragFinished;

        winApiService = new WinApiService();
    }

    private void GlobalHook_MouseDragStarted(object? sender, System.Windows.Forms.MouseEventArgs e)
    {
        Dev.Log();
        isDragging = true;
        element = DragElement.None;
    }

    private void GlobalHook_MouseMove(object? sender, System.Windows.Forms.MouseEventArgs e)
    {
        if (isDragging)
        {
            if (element == DragElement.None)
            {
                var currentWindow = winApiService.GetActiveWindow();

                if (currentWindow != null)
                {
                    if (currentWindow.Boundry.Equals(active.Boundry))
                    {
                        element = DragElement.Tab;
                    }
                    else
                    {
                        element = DragElement.Window;
                    }
                }
            }

            Dev.Log(element);
        }
    }

    private void GlobalHook_MouseDragFinished(object? sender, System.Windows.Forms.MouseEventArgs e)
    {
        Dev.Log();
        isDragging = false;
    }

    private void GlobalHook_MouseDown(object? sender, System.Windows.Forms.MouseEventArgs e)
    {
        if (e.Button == System.Windows.Forms.MouseButtons.Left)
        {
            active = winApiService.GetActiveWindow();
            if (active?.Title != null && active.Title.Contains("firefox", System.StringComparison.InvariantCultureIgnoreCase))
            {
                //isListening = true;
                firefoxes = AutomationElement.RootElement.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "MozillaWindowClass"));

                foreach (AutomationElement item in firefoxes)
                {
                    Dev.Log($"{item.Current.NativeWindowHandle} - {item.Current.Name}");
                }
            }
        }
    }

    private void GlobalHook_MouseUp(object? sender, System.Windows.Forms.MouseEventArgs e)
    {
        if (e.Button == System.Windows.Forms.MouseButtons.Left)
        {
            if (firefoxes != null && firefoxes.Count > 0)
            {
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    await Task.Delay(1000);
                    currentfirefoxes = AutomationElement.RootElement.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "MozillaWindowClass"));

                    AutomationElement found = null;

                    if (firefoxes != null && currentfirefoxes != null && currentfirefoxes.Count > firefoxes.Count)
                    {
                        foreach (AutomationElement item in currentfirefoxes)
                        {
                            Dev.Log($"{item.Current.NativeWindowHandle} - {item.Current.Name}");
                        }

                        var dictionary = new Dictionary<int, AutomationElement>();
                        var currentDictionary = new Dictionary<int, AutomationElement>();

                        foreach (AutomationElement current in currentfirefoxes)
                        {
                            currentDictionary.Add(current.Current.NativeWindowHandle, current);
                        }

                        foreach (AutomationElement item in firefoxes)
                        {
                            dictionary.Add(item.Current.NativeWindowHandle, item);
                        }

                        var dict1 = currentDictionary.Except(dictionary);

                        found = dict1.FirstOrDefault().Value;
                    }

                    Dev.Log($"FOUND : {found?.Current.Name}, {found?.Current.NativeWindowHandle}"); //name can be null here

                    firefoxes = null;
                    currentfirefoxes = null;
                });
            }
        }
    }

    private string GetBrowserURL()
    {
        //var list = AutomationElement.RootElement.FindAll(TreeScope.Children,
        //           new PropertyCondition(AutomationElement.ClassNameProperty, "MozillaWindowClass"));

        //AutomationElement root = AutomationElement.RootElement.FindFirst(TreeScope.Children,
        //    new PropertyCondition(AutomationElement.ClassNameProperty, "MozillaWindowClass"));

        //Condition toolBar = new AndCondition(
        //new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ToolBar),
        //new PropertyCondition(AutomationElement.NameProperty, "Browser tabs"));
        //var tool = root.FindFirst(TreeScope.Children, toolBar);

        //var tool2 = TreeWalker.ControlViewWalker.GetNextSibling(tool);

        //var children = tool2.FindAll(TreeScope.Children, Condition.TrueCondition);

        //foreach (AutomationElement item in children)
        //{
        //    foreach (AutomationElement i in item.FindAll(TreeScope.Children, Condition.TrueCondition))
        //    {
        //        foreach (AutomationElement ii in i.FindAll(TreeScope.Element, Condition.TrueCondition))
        //        {
        //            //if (ii.Current.LocalizedControlType == "edit")
        //            {
        //                //if (!ii.Current.BoundingRectangle.X.ToString().Contains("empty"))
        //                {
        //                    try
        //                    {
        //                        ValuePattern activeTab = ii.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
        //                        var activeUrl = activeTab.Current.Value;
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        return string.Empty;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        GetBrowserURL();
    }
}