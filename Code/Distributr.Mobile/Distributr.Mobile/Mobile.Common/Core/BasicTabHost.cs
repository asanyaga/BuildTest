using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Mobile.Common.Core
{
    public class BasicTabHost : LinearLayout, View.IOnClickListener
    {
        private readonly LayoutParams tabParams = new LayoutParams(0, ViewGroup.LayoutParams.WrapContent, 1.0f);
        private Dictionary<string, Tab> tabs = new Dictionary<string, Tab>();

        public BasicTabHost(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            //tabParams = new LayoutParams((Resources.DisplayMetrics.WidthPixels / 3), ViewGroup.LayoutParams.WrapContent);
        }

        public void OnClick(View view)
        {
            var button = view as Button;
            var name = button.Text;
            SelectTab(name);
        }

        public void AddTab(TabModel model)
        {
            if (tabs.ContainsKey(model.Name))
            {
                return;
            }
            Visibility = ViewStates.Visible;
            var inflator = LayoutInflater.From(Context);
            
            var tab = inflator.Inflate(Resource.Layout.tab, null) as Tab;
            tab.SetTabModel(model);
            tab.SetOnClickListener(this);
            tab.LayoutParameters = tabParams;

            if (tabs.Count == 0)
            {
                tab.OnSelected(false);
            }
            else
            {
                tab.OnDeselected();
            }

            tabs.Add(model.Name, tab);

            AddView(tab);
        }

        private void SelectTab(string name)
        {
            Select(name);
        }

        public void Reset()
        {
            Visibility = ViewStates.Gone;
            RemoveAllViews();
            tabs = new Dictionary<string, Tab>();
        }

        public void Select(string name)
        {
            foreach (var tab in tabs.Values.Where(tab => tabs[name] != tab))
            {
                tab.OnDeselected();
            }
            tabs[name].OnSelected();
            Invalidate();
        }
    }

    public class TabModel
    {
        public string Name { get; private set; }
        public Action OnTabSelected { get; set; }

        public TabModel(string name)
        {
            Name = name;
        }
    }

    public class Tab : LinearLayout
    {
        private TabModel model;
        private Button tabButton;
        public View TabIndicator;
        private Color selectedColor;
        private Color deselectedColor;
        private Color indicatorColor;

        public Tab(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        protected override void OnFinishInflate()
        {
            base.OnFinishInflate();
            for (var i = 0; i < ChildCount; i++)
            {
                var view = GetChildAt(i);
                if (view is Button)
                {
                    tabButton = view as Button;
                }
                else
                {
                    TabIndicator = view;
                }
            }
            indicatorColor = Context.Resources.GetColor(Resource.Color.color_accent);
            selectedColor = Resources.GetColor(Resource.Color.color_action_bar_content);
            //Set deslected color to 80% transparency
            deselectedColor = Color.Argb(204, selectedColor.R, selectedColor.G, selectedColor.B);
        }

        public override void SetOnClickListener(IOnClickListener listener)
        {
            tabButton.SetOnClickListener(listener);
        }

        public void SetTabModel(TabModel model)
        {
            this.model = model;
            tabButton.Text = model.Name;
        }

        public void OnSelected(bool fireModelEvent = true)
        {
            tabButton.SetTextColor(selectedColor);
            TabIndicator.SetBackgroundColor(indicatorColor);
            if (fireModelEvent)
            {
                model.OnTabSelected();
            }
        }

        public void OnDeselected()
        {
            TabIndicator.SetBackgroundColor(Color.Transparent);
            tabButton.SetTextColor(deselectedColor);
        }
    }
}