using System;
using Android.Animation;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Java.Interop;
using Exception = System.Exception;
using Object = Java.Lang.Object;

namespace Mobile.Common.Core.Views
{
    //Based on this https://github.com/ozodrukh/CircularReveal/tree/master/circualreveal
    
    public class RevealFrameLayout : FrameLayout, IRevealAnimator
    {
        public Path RevealPath { get; set; }
        public bool ClipOutlines { get; set; }
        public int CentreX { get; set; }
        public int CentreY { get; set; }

        private float radius;

        [Export]
        public void setRadius(float radius)
        {
            this.radius = radius;
            Invalidate();
        }

        [Export]
        public float getRadius()
        {
            return radius;
        }

        public View Target { get; set; }

        public RevealFrameLayout(Context context) : this(context, null)
        {
        }

        public RevealFrameLayout(Context context, IAttributeSet attrs) : this(context, attrs, 0)
        {
        }

        public RevealFrameLayout(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            RevealPath = new Path();
        }

        protected override bool DrawChild(Canvas canvas, View child, long drawingTime)
        {
            if(!ClipOutlines && Target != null)
                return base.DrawChild(canvas, child, drawingTime);

            var state = canvas.Save();

            RevealPath.Reset();
            RevealPath.AddCircle(CentreX, CentreY, getRadius(), Path.Direction.Cw);
            
            canvas.ClipPath(RevealPath);

            var isInvalid = base.DrawChild(canvas, child, drawingTime);

            canvas.RestoreToCount(state);

            return isInvalid;
        }
    }

    public interface IRevealAnimator
    {
        void Invalidate(Rect bounds);
        Path RevealPath { get; set; }
        bool ClipOutlines { get; set; }
        int CentreX { get; set; }
        int CentreY { get; set; }
        void setRadius(float radius);
        float getRadius();
        View Target { get; set; }
    }

    public class RevealFinishedIceCreamSandwich : Object, Animator.IAnimatorListener
    {
        private readonly WeakReference reference;
        private volatile Rect invalidateBounds;
        private LayerType LayerType { get; set; }

        public RevealFinishedIceCreamSandwich(IRevealAnimator animator, Rect bounds)
        {
            reference = new WeakReference(animator);
            invalidateBounds = bounds;
            LayerType = ((View) animator).LayerType;
        }

        public void OnAnimationStart(Animator animation)
        {
            View view = reference.Target as View;
            view.SetLayerType(LayerType.Software, null);
        }

        public void OnAnimationEnd(Animator animation)
        {
            View view = reference.Target as View;
            view.SetLayerType(LayerType, null);

            var animator = (IRevealAnimator) view;

            if (animator.Target == null) return;

            animator.ClipOutlines = false;
            animator.Target = null;
            animator.Invalidate(invalidateBounds);
        }

        public void OnAnimationRepeat(Animator animation) { }
        public void OnAnimationCancel(Animator animation) { }
    }

    public class RevealFinishedJellyBeanMr2 : Object, Animator.IAnimatorListener
    {
        private readonly WeakReference reference;
        private volatile Rect invalidateBounds;
        private LayerType LayerType { get; set; }

        public RevealFinishedJellyBeanMr2(IRevealAnimator animator, Rect bounds)
        {
            reference = new WeakReference(animator);
            invalidateBounds = bounds;
            LayerType = ((View)animator).LayerType;
        }

        public void OnAnimationStart(Animator animation)
        {
            View view = reference.Target as View;
            view.Visibility = ViewStates.Visible;
            view.SetLayerType(LayerType.Hardware, null);
        }

        public void OnAnimationEnd(Animator animation)
        {
            View view = reference.Target as View;
            view.SetLayerType(LayerType, null);

            var animator = (IRevealAnimator)view;

            if (animator.Target == null) return;

            animator.ClipOutlines = false;
            animator.Target = null;
            animator.Invalidate(invalidateBounds);
        }

        public void OnAnimationRepeat(Animator animation) { }
        public void OnAnimationCancel(Animator animation) { }
    }

    public class XamarinViewAnimationUtils
    {

        public static Animator CreateCircularReveal(View view, int centreX, int centreY, float startRadius,
            float endRadius)
        {
            var api = (int)Build.VERSION.SdkInt;

//            if (api >= 21)
//            {
//                return ViewAnimationUtils.CreateCircularReveal(view, centreX, centreY, startRadius, endRadius);
//            }

            var animator = view.Parent as IRevealAnimator;
            animator.CentreX = centreX;
            animator.CentreY = centreY;
            animator.Target = view;
            animator.ClipOutlines = true;

            var bounds = new Rect();
            view.GetHitRect(bounds);

            var reveal = ObjectAnimator.OfFloat((Object)animator, "Radius", startRadius, endRadius);
            reveal.SetDuration(300);
            reveal.SetInterpolator(new AccelerateDecelerateInterpolator()); 
            reveal.AddListener(CreateRevealFinishedListener(animator, bounds, api));

            return reveal;
        }

        static Animator.IAnimatorListener CreateRevealFinishedListener(IRevealAnimator target, Rect bounds, int api)
        {
            if (api >= 18)
            {
                return new RevealFinishedJellyBeanMr2(target, bounds);
            }
            else if (api >= 14)
            {
                return new RevealFinishedIceCreamSandwich(target, bounds);
            }
            else
            {
                throw new Exception("I don't support API version " + api);
            }
        }
    }
}