using System;
using UnityEngine;
using Player;
using General;

namespace GamePlay.Collectibles
{
    public enum MushroomType
    {
        Default,
        Red,
        Blue,
        Yellow
    }

    public class Mushroom : CollectibleBase
    {
        [Header("Mushroom type")] public MushroomType _mushroomType = MushroomType.Red;

        [Tooltip("The amount of this resource to give the player.")] [SerializeField]
        private int quantity = 1;

        [Header("Visual Prefabs")] [SerializeField]
        private GameObject _redMushroomPrefab;

        [SerializeField] private GameObject _blueMushroomPrefab;

        [SerializeField] private GameObject _yellowMushroomPrefab;

        protected override void Awake()
        {
            if (_mushroomType == MushroomType.Default)
            {
                _mushroomType = MushroomType.Red;
            }

            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            _redMushroomPrefab.SetActive(false);
            _blueMushroomPrefab.SetActive(false);
            _yellowMushroomPrefab.SetActive(false);

            switch (_mushroomType)
            {
                case MushroomType.Red:
                    if (_redMushroomPrefab != null) _redMushroomPrefab.SetActive(true);
                    break;
                case MushroomType.Blue:
                    if (_blueMushroomPrefab != null) _blueMushroomPrefab.SetActive(true);
                    break;
                case MushroomType.Yellow:
                    if (_yellowMushroomPrefab != null) _yellowMushroomPrefab.SetActive(true);
                    break;
                case MushroomType.Default:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void OnCollected() //  < == players inventory or collectibles logic in our case  
        {
            Destroy(gameObject);

            /*
            if (originSpawner != null && originPoint != null)
            {
                originSpawner.RestartSpawn(originPoint);
            }
            */
        }
    }
}