# FeedbackManager Sistemi - Genel Feedback Kontrolcüsü

## Dosyalar
- `Assets/alperen/Scripts/Interaction/FeedbackManager.cs` - Feedback manager scripti
- `Assets/alperen/Prefabs/FX/SoilParticles.prefab` - Toprak partikül efekti prefab'ı

## Özellikler

### Haptic Feedback
- **Kazı feedback**: Kısa titreşim (Intensity: 0.3, Duration: 0.1s)
- **Başarı feedback**: Güçlü titreşim (Intensity: 0.8, Duration: 0.3s, çift pulse)
- **Pickup feedback**: Hafif titreşim (Intensity: 0.2, Duration: 0.1s)
- **Drop feedback**: Hafif titreşim (Intensity: 0.15, Duration: 0.1s)

### Audio Feedback
- **Spatial audio**: 3D ses konumlandırma
- **Audio pool**: 5 AudioSource (performans için)
- **Cooldown**: Dig sesleri için 0.15s cooldown (spam engelleme)
- **Volume**: Global volume kontrolü (1.0 = %100)

### Particle Effects
- **Soil particles**: 15 adet kahverengi toprak partikülü
- **Prefab support**: Özel prefab veya programatik partiküller
- **Physics**: Rigidbody ile gerçekçi düşüş
- **Lifetime**: 2 saniye (ayarlanabilir)
- **Spread**: 0.2 (partikül yayılımı)

## Kurulum

### 1. FeedbackManager GameObject Oluşturun
1. Sahneye boş bir GameObject ekleyin: "FeedbackManager"
2. FeedbackManager.cs component'ini ekleyin

### 2. Inspector Ayarları
```
Haptic Settings:
  Dig Haptic Intensity: 0.3
  Dig Haptic Duration: 0.1
  Success Haptic Intensity: 0.8
  Success Haptic Duration: 0.3

Audio Settings:
  Dig Sound: [Ses dosyası]
  Success Sound: [Ses dosyası]
  Pickup Sound: [Ses dosyası]
  Drop Sound: [Ses dosyası]
  Volume: 1.0

Particle Effects:
  Soil Particle Prefab: SoilParticles.prefab
  Soil Particle Count: 15
  Particle Lifetime: 2.0
  Particle Spread: 0.2

Bone Socket Connection:
  Bone Socket System: [BoneSocketSystem component'i]

Audio Source Pool:
  Audio Source Pool Size: 5
```

### 3. Ses Dosyalarını Ekleme (Opsiyonel)
```
Assets/alperen/Audio/ klasörüne:
- Dig_Sound.wav
- Success_Sound.wav
- Pickup_Sound.wav
- Drop_Sound.wav
```

### 4. BoneSocketSystem ile Bağlama
BoneSocketSystem otomatik olarak bulunur (FindObjectOfType).

Manuel atama için:
1. BoneSocketSystem prefab'ı seçin
2. OnBoneSocketed event'ini açın
3. "+" tıklayın
4. FeedbackManager GameObject'ini sürükleyin
5. No Function > FeedbackManager > TriggerSuccessFeedback seçin

## API Kullanımı

### Singleton Erişim
```csharp
// Global instance'a eriş:
FeedbackManager.Instance.TriggerDigFeedback(position);
```

### Kazı Feedback
```csharp
// Kazı noktasında feedback tetikle:
Vector3 digPosition = contact.point;
FeedbackManager.Instance.TriggerDigFeedback(digPosition);

// Haptic + Audio + Particles tetiklenir
```

### Başarı Feedback
```csharp
// Başarı feedback tetikle:
FeedbackManager.Instance.TriggerSuccessFeedback();

// Güçlü haptic + başarı sesi tetiklenir
```

### Pickup/Drop Feedback
```csharp
// Nesne alındığında:
FeedbackManager.Instance.TriggerPickupFeedback();

// Nesne bırakıldığında:
FeedbackManager.Instance.TriggerDropFeedback();
```

### Manuel Controller Atama
```csharp
// XR Controller'ı manuel atayın:
XRController controller = GetComponent<XRController>();
FeedbackManager.Instance.SetActiveController(controller);
```

### Haptic Intensity Ayarlama
```csharp
// Kazı haptic intensity'ini ayarla:
FeedbackManager.Instance.SetDigHapticIntensity(0.5f);

// Başarı haptic intensity'ini ayarla:
FeedbackManager.Instance.SetSuccessHapticIntensity(1.0f);
```

### Volume Ayarlama
```csharp
// Global volume ayarla:
FeedbackManager.Instance.SetVolume(0.7f); // %70

// Mevcut volume al:
float volume = FeedbackManager.Instance.Volume;
```

## Event Sistemi

### FeedbackManager Events
```csharp
// Kazı feedback eventi:
FeedbackManager.Instance.OnDigFeedback += () => {
    Debug.Log("Kazı feedback tetiklendi!");
};

// Başarı feedback eventi:
FeedbackManager.Instance.OnSuccessFeedback += () => {
    Debug.Log("Başarı feedback tetiklendi!");
};
```

### Diğer Sistemlerle Bağlantı

#### DiggingSystem → FeedbackManager
```csharp
// DiggingSystem.cs PerformDig() fonksiyonunda:
private void PerformDig(Vector3 contactPoint, Vector3 contactNormal)
{
    lastDigTime = Time.time;
    Debug.Log($"Kazı yapıldı - Pozisyon: {contactPoint}");
    SpawnSoilParticles(contactPoint, contactNormal);

    // FeedbackManager ile entegrasyon:
    FeedbackManager.Instance.TriggerDigFeedback(contactPoint);

    OnDig?.Invoke(contactPoint);
}
```

#### BoneSocketSystem → FeedbackManager
```csharp
// BoneSocketSystem.cs OnBoneSocketEnter() fonksiyonunda:
private void OnBoneSocketEnter(SelectEnterEventArgs args)
{
    GameObject boneObject = args.interactableObject.transform.gameObject;

    if (!boneObject.CompareTag(acceptedTag))
    {
        return;
    }

    currentBoneCount++;
    Debug.Log($"Kemik yerleştirildi! {boneObject.name}");

    OnBoneSocketed?.Invoke(boneObject);
    UpdateVisualFeedback(true);
    TriggerHapticFeedback(args);

    // FeedbackManager ile entegrasyon:
    FeedbackManager.Instance.TriggerSuccessFeedback();
}
```

## SoilParticles Prefap

### Prefap Özellikleri
- **Particle System**: Unity Particle System component'i
- **Burst Mode**: 20 particle tek seferde
- **Lifetime**: 1.5 saniye
- **Color**: Kahverengi (0.4, 0.3, 0.2) to (0.5, 0.35, 0.2)
- **Gravity**: Yerçekimi ile düşüş
- **Shape**: Hemisphere (yukarı doğru yayılma)
- **Size**: 0.05-0.15 (rastgele)
- **Speed**: 1-3 (rastgele)

### Manuel Particle Oluşturma
Prefab atanmazsa, FeedbackManager programatik olarak partikül oluşturur:
- 15 adet cube mesh
- Rastgele kahverengi renkler
- Rigidbody ile fizik
- Rastgele rotasyon
- 2 saniye lifetime

## Performans İpuçları

### Audio Pool Kullanımı
- AudioSource'ler yeniden kullanılır (destroy edilmez)
- Pool size: 5 (önerilen)
- Spatial audio: Linear falloff, 20m range

### Particle Optimizasyonu
- Particle count: 10-15 (önerilen)
- Lifetime: 1.5-2.0s (önerilen)
- Programatik partiküller: Basit mesh (cube)
- Mesh renderer kullanın (SkinnedMeshRenderer yok)

### Haptic Cooldown
- Dig sounds: 0.15s cooldown
- Spam engelleme için önemli
- Performans için AudioSource'ları kullanmayın yok edin

## Test Etme

### Haptic Test
1. Play mod başlat
2. VR headset bağlayın
3. Shovel ile toprağa dokunun
4. Haptic hissediniz mi?
5. Kemik yuvaya yerleştirin
6. Güçlü haptic hissediniz mi?

### Audio Test
1. Play mod başlat
2. Shovel ile kazı yapın
3. Kazı sesi çalıyor mu?
4. Kemik yuvaya yerleştirin
5. Başarı sesi çalıyor mu?

### Particle Test
1. Play mod başlat
2. Shovel ile toprağa dokunun
3. Partikül efektleri görüyor musunuz?
4. Partiküller düşüyor mu?
5. Partiküller yok oluyor mu (2s)?

## Hata Giderme

### Haptics Çalışmıyor
```
Çözüm 1: XR Controller'ı manuel atayın
FeedbackManager.Instance.SetActiveController(controller);

Çözüm 2: Haptic intensity'yi artırın
FeedbackManager.Instance.SetDigHapticIntensity(0.8f);
```

### Audio Çalmıyor
```
Çözüm 1: Ses dosyası atanmış mı kontrol edin
Çözüm 2: Volume kontrol edin
FeedbackManager.Instance.SetVolume(1.0f);
Çözüm 3: AudioSource pool kontrol edin
```

### Partikül Görünmüyor
```
Çözüm 1: Particle system enabled kontrol edin
Çözüm 2: Renderer enabled kontrol edin
Çözüm 3: Camera layer mask kontrol edin
Çözüm 4: Prefab atanmamısa programatik kontrol edin
```

### Cooldown Çok Uzun
```
Çözüm: DigCooldown değerini düşürün
// FeedbackManager.cs içinde:
float digCooldown = 0.15f; // 0.10f yapabilirsiniz
```

## SSS

### S: Neden Singleton pattern kullanılıyor?
A: FeedbackManager'a global erişim için (tüm sistemlerden çağrılabilir).

### S: Neden Audio pool kullanılıyor?
A: AudioSource'ler her ses için oluşturulup destroy edilirse performans düşer. Pool ile yeniden kullanılır.

### S: Partiküller neden programatik oluşturuluyor?
A: Prefab atanmazsa otomatik yedek olarak çalışması için. Prefab atanırsa daha performanslı.

### S: Haptic intensity ne olmalı?
A: Dig: 0.2-0.4, Success: 0.6-1.0. Cihaza göre ayarlanabilir.

### S: Volume nasıl ayarlanır?
A: FeedbackManager.Instance.SetVolume(0.5f) veya Inspector'dan.

## Önerilen Ayarlar

### VR Controller (Quest 2/3)
```
Dig Haptic Intensity: 0.3
Success Haptic Intensity: 0.8
Dig Duration: 0.1s
Success Duration: 0.3s
```

### VR Controller (Vive/Index)
```
Dig Haptic Intensity: 0.5
Success Haptic Intensity: 1.0
Dig Duration: 0.15s
Success Duration: 0.4s
```

### Audio
```
Volume: 0.8
Dig Sound: Short, punchy
Success Sound: Clear, rewarding
Pickup/Drop Sound: Subtle
```

### Particles
```
Soil Particle Count: 15
Particle Lifetime: 2.0s
Particle Spread: 0.2
Gravity Modifier: 0.5
```

## Sonraki Geliştirmeler

- [ ] Custom particle prefabs için daha fazla efekt
- [ ] Haptic pattern'leri (ritmik titreşimler)
- [ ] Audio spatialization (HRTF)
- [ ] Particle mesh varyasyonları (farklı şekiller)
- [ ] Particle color varyasyonları (farklı toprak türleri)
