using System;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Distributr.Mobile.Products
{
    //TODO clean up code
    public class AddProductItemView : LinearLayout, View.IOnClickListener
    {
        private Color highlightText;
        private UIProduct product;
        private TextView productName;
        private EditText quantity;
        private bool quantityPaddingSet;
        private TextView returnable;
        private bool returnableMode;
        private TextView returnablePrice;
        private Color standardText;

        public AddProductItemView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public ProductStateChangedListener OnStateChangeListener { private get; set; }

        public void OnClick(View view)
        {
            switch (view.Id)
            {
                case Resource.Id.quantity_backspace:
                    HandleQuantityBackspacePressed();
                    break;
                case Resource.Id.case_button:
                    HandleUnitTypeChange(Resource.Id.case_button);
                    break;
                case Resource.Id.each_button:
                    HandleUnitTypeChange(Resource.Id.each_button);
                    break;
                case Resource.Id.returnable:
                    SwitchToReturnableMode();
                    break;
                case Resource.Id.product_name:
                    SwitchToProductMode();
                    break;
                default:
                    var button = view as Button;
                    AppendToQuantity(button.Text);
                    break;
            }

            FireStateChanged();
        }

        public void UpdateState(UIProduct product)
        {
            this.product = product;

            if (product.Quantity > 0)
            {
                quantity.Text = product.Quantity.ToString();
            }
            else
            {
                quantity.Text = string.Empty;
            }

            productName.Text = product.Name;

            if (product.HasReturnables)
            {
                returnable.Visibility = ViewStates.Visible;
                returnablePrice.Visibility = ViewStates.Visible;
            }
            else
            {
                returnable.Visibility = ViewStates.Gone;
                returnablePrice.Visibility = ViewStates.Gone;
            }

            if (product.UnitType == UnitType.Each)
            {
                HandleUnitTypeChange(Resource.Id.each_button);
            }
            else
            {
                HandleUnitTypeChange(Resource.Id.case_button);
            }

            SwitchToProductMode();
        }

        protected override void OnFinishInflate()
        {
            base.OnFinishInflate();
            SetupViews();
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);
            if (!quantityPaddingSet)
            {
                var quantityBackspace = FindViewById<ImageButton>(Resource.Id.quantity_backspace);
                quantity.SetPadding(quantity.PaddingLeft, quantity.PaddingTop, quantityBackspace.Width,
                    quantity.PaddingBottom);
                quantityPaddingSet = true;
            }
        }

        private void SetupViews()
        {
            quantity = FindViewById<EditText>(Resource.Id.quantity);
            productName = FindViewById<TextView>(Resource.Id.product_name);
            productName.SetOnClickListener(this);

            returnable = FindViewById<TextView>(Resource.Id.returnable);
            returnable.SetOnClickListener(this);

            returnablePrice = FindViewById<TextView>(Resource.Id.returnable_price);

            var backspace = FindViewById<ImageButton>(Resource.Id.quantity_backspace);
            backspace.SetOnClickListener(this);

            highlightText = Resources.GetColor(Resource.Color.colorPrimary);
            standardText = Resources.GetColor(Resource.Color.primaryText);

            AddListenterToButtons(this);
        }

        private void SwitchToProductMode()
        {
            returnableMode = false;
            SetQuantity(product.Quantity);
            productName.SetTextColor(highlightText);
            returnable.SetTextColor(standardText);
        }

        private void SetQuantity(long quantity)
        {
            if (quantity != 0)
            {
                this.quantity.Text = quantity.ToString();
            }
            else
            {
                this.quantity.Text = string.Empty;
            }
        }

        private void SwitchToReturnableMode()
        {
            returnableMode = true;
            SetQuantity(product.ReturnableQuantity);
            productName.SetTextColor(standardText);
            returnable.SetTextColor(highlightText);
        }

        private void HandleQuantityBackspacePressed()
        {
            if (!string.IsNullOrEmpty(quantity.Text))
            {
                var text = quantity.Text;
                quantity.Text = text.Substring(0, text.Length - 1);
                UpdateProduct(quantity.Text);
            }
        }

        private void AddListenterToButtons(ViewGroup parent)
        {
            for (var i = 0; i < parent.ChildCount; i++)
            {
                var child = parent.GetChildAt(i);
                if (child is ViewGroup)
                {
                    AddListenterToButtons(child as ViewGroup);
                }
                else if (child is Button)
                {
                    child.SetOnClickListener(this);
                }
            }
        }

        private void FireStateChanged()
        {
            OnStateChangeListener.OnStateChanged(product);
        }

        private void AppendToQuantity(string value)
        {
            quantity.Text += value;
            UpdateProduct(quantity.Text);
        }

        private void UpdateProduct(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                if (returnableMode)
                {
                    product.ReturnableQuantity = 0;
                }
                else
                {
                    product.Quantity = 0;
                }
            }
            else
            {
                if (returnableMode)
                {
                    product.ReturnableQuantity = Convert.ToInt64(text);
                }
                else
                {
                    product.Quantity = Convert.ToInt64(text);
                }
            }
        }

        private void HandleUnitTypeChange(int viewId)
        {
            if (viewId == Resource.Id.each_button)
            {
                FindViewById<Button>(viewId).SetTextColor(highlightText);
                FindViewById<Button>(Resource.Id.case_button).SetTextColor(standardText);
                product.UnitType = UnitType.Each;
            }
            else
            {
                FindViewById<Button>(viewId).SetTextColor(highlightText);
                FindViewById<Button>(Resource.Id.each_button).SetTextColor(standardText);
                product.UnitType = UnitType.Case;
            }
        }
    }

    public interface ProductStateChangedListener
    {
        void OnStateChanged(UIProduct product);
    }
}