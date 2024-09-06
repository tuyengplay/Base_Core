using EranCore;
using EranCore.Audio;
using EranCore.Tweening;
using EranCore.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace GameCore
{
    public class BuyManager : Singleton<BuyManager>
    {
        List<ItemValueFloat> requires = null;
        Action<bool> OnDone = null;
        public void Buy(List<ItemValueFloat> _requires, Action<bool> _OnDone, string _adLocal)
        {
            requires = _requires;
            OnDone = _OnDone;
            if (_requires == null || requires.Count <= 0)
            {
                OnCheckDone(true);
                return;
            }
            if (requires.Contains(x => x.ID == ItemID.Ads))
            {
                OnADS(_adLocal, OnCheckDone);
            }
            else if (requires.Contains(x => ItemType.IsIapPack(x.ID)))
            {
                //IAPManager.Instance.PurchaseProduct(requires[0].ID, OnCheckDone, _adLocal);
            }
            else
            {

                foreach (ItemValueFloat require in requires)
                {
                    int userValue = User.Ins[require.ID];
                    if (userValue < require.Value)
                    {
                        OnCheckDone(false, require.ID);
                        return;
                    }
                }
                OnCheckDone(true);
            }

        }
        void OnCheckDone(bool isSuccess, ItemID failItem = ItemID.None)
        {
            if (isSuccess)
            {
                StartCoroutine(OnClaimRewards());
            }
            else
            {
                OnFail(failItem);
            }

            requires = null;
            OnDone?.Invoke(isSuccess);
        }
        private void OnADS(string _adLocal, Action<bool, ItemID> onCkeckDone)
        {
            //AdsManager.Instance.ShowRewardedAd(_adLocal.ToString(), (success) =>
            //{
            //    onCkeckDone?.Invoke(success, ItemID.Ads);
            //});
        }
        IEnumerator OnClaimRewards()
        {

            if (requires != null)
            {
                requires.ForEach(x => User.Ins[x.ID] -= (int)x.Value);
            }
            yield break;
        }
        private void OnFail(ItemID failItem)
        {
            Debug.Log("Faild");
            if (failItem == ItemID.Ads)
            {
                //UIManager.Instance.OpenUI<PopupNoti>(StaticClass.PopupNoti, (pop) =>
                //{
                //    pop.Setup("ADS NOT READY!", "Please ,check your internet connection OR watch full video!");
                //});
            }
            else if (ItemType.IsIapPack(failItem))
            {
                //UIManager.Instance.OpenUI<PopupNoti>(StaticClass.PopupNoti, (pop) =>
                //{
                //    pop.Setup("BUY FAIL!", "Please try again!");
                //});
            }
            else
            {
                //UIManager.Instance.OpenUI<PopupNoti>(StaticClass.PopupNoti, (pop) =>
                //{
                //    pop.Setup("BUY FAIL!", $"Please earn more {failItem.ToString()}");
                //});
            }

        }
        #region FXClaim
        public static void ClaimFlyItem(ItemValueInt _item, Transform _startPos, UnityAction _OnComplete, Transform _endPos = null, bool realClaim = true)
        {
            int fxCount;// Tong fx Sinh ra
            int fxValue;// Value trong 1 FX
            int fxEndValue;// Le So Value Con lai
            int maxFxValue = MaxFxValue(_item.ID); // Max Value Trong Fx
            if (_item.Value < maxFxValue)
            {
                fxCount = 1;
                fxValue = 0;
                fxEndValue = _item.Value;
            }
            else
            {
                fxCount = _item.Value / maxFxValue;
                fxValue = maxFxValue;
                fxEndValue = _item.Value % fxValue + fxValue;
            }
            if (fxCount > 30)
            {
                fxCount = 30;
                fxValue = _item.Value / fxCount;
                fxEndValue = _item.Value % fxValue + fxValue;
            }
            Vector3 endPos = Vector3.zero;
            switch (_item.ID)
            {
                case ItemID.Coin:
                    {
                        endPos = TopBar.Instance.posCoin.position;
                        break;
                    }
                default:
                    {
                        endPos = _endPos.position;
                        break;
                    }

            }
            BuyManager.Instance.StartCoroutine(OnShowFlyItem(_item.ID, _startPos.position, endPos, fxCount, () =>
            {
                if (realClaim)
                {
                    User.Ins[_item.ID] += fxValue;
                }
            },
            () =>
            {
                if (realClaim)
                {
                    User.Ins[_item.ID] += fxEndValue;
                }
                _OnComplete?.Invoke();
            }));
        }
        private static int MaxFxValue(ItemID idFX)
        {
            switch (idFX)
            {
                case ItemID.Coin:
                    {
                        return 2;
                    }
            }

            return 10;
        }
        private static IEnumerator OnShowFlyItem(ItemID _id, Vector3 _startPos, Vector3 _endPos, int _coutItem, UnityAction _OnEachComplete, UnityAction _OnLastComplete)
        {
            List<ItemIcon> listITem = new List<ItemIcon>();
            int moveDownCount = 0;
            float randomRadius = 1.2f;
            for (int i = 0; i < _coutItem; i++)
            {
                ItemIcon itemIcon = ContenManager.Emit(ItemID.FXItem) as ItemIcon;
                itemIcon.SetData(_id);
                itemIcon.transform.SetParent(ItemFX.Instance.transform);
                itemIcon.transform.position = _startPos;
                itemIcon.transform.localScale = Vector3.one;
                listITem.Add(itemIcon);
            }
            for (int i = 0; i < listITem.Count; i++)
            {
                listITem[i].transform.TweenMove(_startPos + new Vector3(UnityEngine.Random.Range(randomRadius, -randomRadius), UnityEngine.Random.Range(-randomRadius, randomRadius), 0), 0.3f)
                    .SetDelay(UnityEngine.Random.Range(0.2f, 0.5f)).OnComplete(() => { moveDownCount++; });
            }
            yield return new WaitUntil(() => moveDownCount == listITem.Count);
            int completeItem = 0;
            for (int i = 0; i < listITem.Count; i++)
            {
                listITem[i].transform.TweenMove(_endPos, UnityEngine.Random.Range(0.1f, 1.2f)).SetDelay(UnityEngine.Random.Range(0, 0.5f)).SetEase(Ease.InExpo).OnComplete(() =>
                {
                    if (completeItem != listITem.Count - 1)
                    {
                        _OnEachComplete?.Invoke();
                    }
                    //AudioManager.Instance.PlaySoundUI();
                    completeItem++;
                });
            }
            yield return new WaitUntil(() => completeItem == listITem.Count);
            listITem.ForEach(x => x.Kill());
            _OnLastComplete?.Invoke();
        }
        #endregion
    }
}