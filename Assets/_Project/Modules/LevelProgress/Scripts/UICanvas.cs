using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace DailyReward
{
    public class UICanvas : MonoBehaviour
    {
        private const float ALPHA_ONENABLE = .98f;
        private const float TIME_ANIM_FADE = 0.4f;

        [SerializeField] private Image _backGround;

        private Tween _tweenBackground;



        protected virtual void OnEnable()
        {
            Debug.Log("Pause Game from UICanvas");

            //GameManager.instance?.PauseGame();  // Đặt ở cuối hoặc đầu – nhưng tách khỏi check background

            if (_backGround != null)
            {
                Color color = _backGround.color;
                color.a = 0;
                _backGround.color = color;

                _tweenBackground?.Kill();
                _tweenBackground = _backGround.DOFade(ALPHA_ONENABLE, TIME_ANIM_FADE);

            }
        }

        void Awake()
        {
        }
        protected virtual void OnDisable()
        {
            _tweenBackground?.Kill();
        }
        public void CloseUI()
        {
            BeforeCloseUI(null);
        }
        protected virtual void BeforeCloseUI(Action action)
        {
            //GameManager.instance?.ResumeGame();

            if (!_backGround)
            {
                gameObject.SetActive(false);
                return;
            }

            Color color = _backGround.color;
            color.a = 1;
            _backGround.color = color;

            _tweenBackground?.Kill();

            _tweenBackground = _backGround.DOFade(0, 0.15f).OnComplete(() =>
            {
                if (action != null)
                    action();
                // else
                //     GameManager.Instance.SetPauseGame(false);

                gameObject.SetActive(false);
            });
        }
    }
}
