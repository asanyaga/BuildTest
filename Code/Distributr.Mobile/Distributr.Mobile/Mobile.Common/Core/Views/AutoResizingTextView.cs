using Android.Content;
using Android.Util;
using Android.Widget;

namespace Mobile.Common.Core.Views
{
    // Tries to maintain an equal width for textviews, regardless of the number of characters. 
    // it uses 5 characters as a baseline and reduces the text size by 10% for each additional character
    // over the baseline e.g. if the text becomes 7 characters long it will reduce the original textsize by 20%. 
    public class AutoResizingTextView : TextView
    {
        private float originalSize;
        private const float ReductionFactor = 0.11f;

        public AutoResizingTextView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        protected override void OnFinishInflate()
        {
            SetupAutoResize();
        }

        private void SetupAutoResize()
        {
            originalSize = TextSize;
            AfterTextChanged += delegate
            {
                if (!string.IsNullOrEmpty(Text) && Length() > 5)
                {
                    var reductionAmount = originalSize * ReductionFactor * (Length() - 5);
                    SetTextSize(ComplexUnitType.Px, originalSize - reductionAmount);
                }
                else
                {
                    SetTextSize(ComplexUnitType.Px, originalSize);
                }
            };            
        }
    }
}