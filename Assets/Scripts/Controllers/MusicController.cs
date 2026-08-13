using UnityEngine;
using DG.Tweening;
using System.Collections;

public class MusicController : Singleton<MusicController>
{
    [Header("Music")]
    [SerializeField] private AudioSource _battleMusicSource;
    [SerializeField] private AudioSource _shopMusicSource;

    [SerializeField] private AudioClip _battleClipIntro;
    [SerializeField] private AudioClip _battleClipLoop;

    [SerializeField] private float _battleMusicBaseVolume;
    [SerializeField] private float _shopMusicBaseVolume;

    [Header("Sound Effects")]
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioClip _shootSfx;
    [SerializeField] private AudioClip _hurtSfx;
    [SerializeField] private AudioClip _coinSfx;
    [SerializeField] private AudioClip _powerupSfx;
    [SerializeField] private AudioClip _gameOverSfx;

    private void OnEnable()
    {
        StartCoroutine(PlayBattleMusic());
    }

    private IEnumerator PlayBattleMusic()
    {
        // prepare battle loop so it doesn't delay the first time :(
        _battleMusicSource.volume = 0f;
        _battleMusicSource.clip = _battleClipLoop;
        _battleMusicSource.Play();
        yield return null;
        _battleMusicSource.Stop();

        _battleMusicSource.volume = _battleMusicBaseVolume;
        _battleMusicSource.clip = _battleClipIntro;
        _battleMusicSource.loop = false;
        _battleMusicSource.Play();
        yield return new WaitForSeconds(_battleClipIntro.length);
        _battleMusicSource.clip = _battleClipLoop;
        _battleMusicSource.loop = true;
        _battleMusicSource.Play();
    }

    public void FadeInShopMusic()
    {
        if (_shopMusicSource.isPlaying == false)
        {
            _shopMusicSource.Play();
        }

        _battleMusicSource.DOFade(0f, 2.5f).OnComplete(() =>
        {
            _shopMusicSource.DOFade(_shopMusicBaseVolume, 1f);
        });
    }

    public void FadeInBattleMusic()
    {
        _shopMusicSource.DOFade(0f, 1f).OnComplete(() =>
        {
            _battleMusicSource.DOFade(_battleMusicBaseVolume, 1f);
        });
    }

    public void FadeOutBattleMusic()
    {
        _battleMusicSource.DOFade(0f, 2f);
    }

    public void PlayShootSfx()
    {
        _sfxSource.PlayOneShot(_shootSfx);
    }

    public void PlayHurtSfx()
    {
        _sfxSource.PlayOneShot(_hurtSfx);
    }

    public void PlayGameOverSfx()
    {
        _sfxSource.PlayOneShot(_gameOverSfx);
    }

    public void PlayCoinSfx()
    {
        _sfxSource.PlayOneShot(_coinSfx);
    }

    public void PlayPowerupSfx()
    {
        _sfxSource.PlayOneShot(_powerupSfx);
    }
}
