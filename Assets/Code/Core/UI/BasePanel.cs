using System;
using Code.Core.Extensions;
using Code.Core.UI.Utility;
using Core.Extensions;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Core.UI
{
    public abstract class BasePanel : BaseUI
    {
        public bool Closed { get; protected set; }
        public bool allowToggleWeekGuide = true;
        protected virtual string ContentNodeName => "Content";
        protected const string     MaskNodeName = "Mask";
        protected       Button     _maskButton;

        public Action<BasePanel> OnBeforeShow;
        public Action<BasePanel> OnAfterShow;
        public Action<BasePanel> OnBeforeClose;
        public Action<BasePanel> OnAfterClose;

        public virtual void Initialize(params object[] p)
        {
        }

        public virtual bool IsHidden
        {
            get
            {
                if (!gameObject) return true;
                return !gameObject.activeSelf;
            }
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void Reshow()
        {
            gameObject.SetActive(true);
        }

        protected virtual void PlayOpenSound()
        {
            //AudioManager.PlaySound("G_BOARD_POP");
        }

        public virtual void Popup()
        {
            PlayOpenSound();
            var content = transform.Find(ContentNodeName);
            CanvasGroup canvasGroup = null;
            if (content)
            {
                canvasGroup = ComponentHolderProtocol.GetOrAddComponent<CanvasGroup>(content.gameObject);
                canvasGroup.alpha = 0;
            }
            
            //delay一帧防卡顿
            Timeout.SetDelay(0, () =>
            {
                if (content)
                {
                    content.localScale = Vector3.one * 0.9f;
                    var seq = DOTween.Sequence();
                    seq.Append(content.DOScale(1.02f, 0.15f));
                    seq.Append(content.DOScale(1f, 0.15f));
                    seq.Insert(0, canvasGroup.DOFade(1, 0.15f).SetEase(DoTweenExtension.EaseOut(2.0f)));
                    seq.OnComplete(() =>
                    {
                        if (this)
                        {
                            OnAfterShow?.Invoke(this);
                        }
                    });
                    seq.Play();
                    seq.SetTarget(gameObject);
                }
                else
                {
                    OnAfterShow?.Invoke(this);
                }
            }, this);
        }

        public virtual void Reset()
        {
            Closed        = false;
            OnBeforeClose = null;
            OnAfterClose  = null;
            OnBeforeShow  = null;
            OnAfterShow   = null;
        }

        ///不要手动调用Close方法，应该调用ClosePanel方法，否则会导致UIManager中panelList未删除引用！
        public virtual void Close(bool caching)
        {
            if (Closed)
            {
                OnAfterClose?.Invoke(this);
                return;
            }
            
            DOTween.Kill(gameObject);
            Closed = true;
            OnBeforeClose?.Invoke(this);
            
            var content = transform.Find(ContentNodeName);
            var canvasGroup = ComponentHolderProtocol.GetOrAddComponent<CanvasGroup>(gameObject);
            var seq = DOTween.Sequence();
            if (content)
            {
                content.localScale = Vector3.one;
                seq.Insert(0, content.DOScale(0.75f, 0.1f));
            }
            

            seq.Insert(0,canvasGroup.DOFade(0, 0.1f).SetEase(DoTweenExtension.EaseOut(2.0f)));
            seq.Play();
            seq.onComplete += () =>
            {
                if (caching)
                {
                    gameObject.SetActive(false);
                    canvasGroup.alpha = 1;
                    if (content) content.localScale = Vector3.one;
                }
                else
                {
                    Destroy(gameObject);
                }

                OnAfterClose?.Invoke(this);
            };
        }

        public virtual void ClosePanel()
        {
            //Code.Core.UI.UIManager.ClosePanel(this);
        }

        public virtual void CreateMask()
        {
            if (_maskButton != null) return;
            //_maskButton = UiUtility.CreateMask(MaskNodeName, transform, OnMaskClicked, 0.15f);
        }

        protected virtual void OnMaskClicked()
        {
            _maskButton.onClick.RemoveListener(OnMaskClicked);
            //Code.Core.UI.UIManager.ClosePanel(this);
        }
    }
}