using System.Linq;
using UnityEngine.Pool;
using UnityUtils.Editor;

namespace UnityUtils.GizmosDrawers.Editor
{
    /// <summary>
    /// Note:
    /// Dear reader, AKA future me, you might be asking - why on earth are we doing this.
    /// Instead of just making a god damn editor window.
    ///
    /// The idea is that we don't want yet another editor window running around.
    /// In the ideal world, we really want this to be embedded in our existing debug window, otherwise there will just be too much around.
    /// The way we do this now allows us to do exactly that - whoever(in our case, the game editor code) can draw the window however he wants, and then chuck this in.
    /// So it works almost like a plugin.
    ///
    /// Oh then you might be thinking:
    /// "Woah dude, that's all well and good, but why on earth do you not just expose the state to public, the application can handle the draw code"
    /// Which again, is a fair question.
    /// But that will lead to our API being a little bit muddied by a bunch of states code that shouldn't be access ordinarily, which I don't really like.
    /// We can technically hide them in in the Intellisense via attributes, but that's annoying in its own way.
    /// So here we go, I hope you like my thought process - but if you don't like it, please do invent a time machine so you can let me know.  
    /// </summary>
    public static class GizmosDrawerEditorExtensions
    {
        public static void DrawGizmosDrawerSettings(this NonebGUIDrawer drawer)
        {
            using (HashSetPool<string>.Get(out var allCategories))
            {
                foreach (var request in GizmosDrawer.Requests)
                {
                    allCategories.Add(request.RequestCategory);
                }

                using (drawer.HorizontalScope())
                {
                    if (drawer.DrawButton("ShowAll"))
                    {
                        GizmosDrawer.ShowAll();
                    }

                    if (drawer.DrawButton("HideAll"))
                    {
                        GizmosDrawer.HideAll();
                    }

                    if (drawer.DrawButton("DismissAll"))
                    {
                        GizmosDrawer.DismissAll();
                    }
                }

                using (drawer.FlowLayoutScope())
                {
                    using (drawer.BoxScope("Visible Categories"))
                    {
                        foreach (var category in allCategories)
                        {
                            var prevIsFiltered = GizmosDrawerPreferences.instance.FilteredCategories.Contains(category);
                            var prevIsVisible = !prevIsFiltered;
                            
                            var isUserWantVisible = drawer.DrawToggle(category, prevIsVisible);
                            var newIsFiltered = !isUserWantVisible;
                            
                            if (newIsFiltered == prevIsFiltered)
                            {
                                continue;
                            }

                            GizmosDrawerPreferences.instance.SetFilter(category, newIsFiltered);
                        }
                    }
                }
            }
        }
    }
}