# VR Arkeolojik Kazı Projesi - Proje Özeti

## Proje Açıklaması
1 haftalık VR arkeolojik kazı simülasyonu projesi. Kullanıcı karanlık bir mağarada dinozor kemikleri kazıp tablet üzerinden bilgi alır.

## Teknoloji Stack
- **Engine**: Unity 2022.3.62f3
- **Language**: C#
- **VR Framework**: XR Interaction Toolkit (XRI) 3.1.2
- **Rendering**: URP (Universal Render Pipeline) 14.0.12
- **Input System**: Unity Input System 1.14.0
- **XR**: OpenXR 1.14.3

## Proje Yapısı

```
Assets/alperen/
├── Prefabs/
│   ├── Tools/
│   │   ├── Shovel.prefab               -- Kazı küreği
│   │   ├── Flashlight.prefab           -- El feneri
│   │   └── XROrigin.prefab            -- VR origin
│   ├── Collectibles/
│   │   └── DinoBone.prefab            -- Dinozor kemikleri
│   ├── Environment/
│   │   └── BoneSocket.prefab          -- Kemik yuvası
│   ├── UI/
│   │   └── InfoTablet.prefab          -- Bilgi tableti
│   └── FX/
│       └── SoilParticles.prefab        -- Toprak partikül efekti
├── Scripts/
│   ├── Interaction/
│   │   ├── DiggingSystem.cs           -- Kazı mekanik sistemi
│   │   ├── BoneSocketSystem.cs        -- Kemik yuvası sistemi
│   │   ├── DinoBoneTagFilter.cs       -- Tag filtresi
│   │   ├── FlashlightController.cs     -- Fener kontrolcüsü
│   │   └── FeedbackManager.cs         -- Genel feedback sistemi
│   ├── UI/
│   │   ├── TabletManager.cs            -- Tablet yöneticisi
│   │   └── HologramController.cs       -- Hologram efekti
│   ├── Environment/
│   │   └── AtmosphereManager.cs        -- Mağara atmosferi
│   └── AlperenManager.cs             -- Ana kontrolcü
└── Scenes/
    └── ExcavationCave.unity          -- Ana sahne
```

## Tamamlanan Görevler

### S1-01: XR Origin Kurulumu ✅
- XR Interaction Toolkit kurulu (3.1.2)
- XR Origin prefab oluşturuldu (kamera + eller)
- Sahneye yerleştirildi

### S1-02: Shovel Prefab ✅
- XR Grab Interactable ile tutulabilir
- DiggingSystem.cs ile kazı mekanikleri
- Toprakla temas algılama
- Haptic feedback entegrasyonu

### S1-03: Kazı Mekaniği ✅
- Soil Layer kontrolü (Layer 6)
- Hız kontrolü (minDigVelocity: 0.5)
- Cooldown sistemi (0.2s)
- Partikül efektleri (15 adet)

### S1-04: Bone Socket Sistemi ✅
- XRSocketInteractor ile yuva sistemi
- "DinoBone" tag filtresi
- Event sistemi (OnBoneSocketed)
- Görsel feedback (renk değişimi)

### S1-05: Flashlight Sistemi ✅
- XR Grab Interactable ile tutulabilir
- Spotlight (Intensity: 2, Range: 15)
- Trigger tuşu ile açma/kapama
- Haptic feedback entegrasyonu

### S1-06: Mağara Atmosferi ✅
- Karanlık ortam (Ambient: 0.05)
- Sis efekti (Density: 0.05)
- Ambient point lights (3 adet)
- Runtime ayar değişimi desteği

### S1-07: Tablet Sistemi ✅
- Canvas tabanlı UI
- Dinozor bilgileri gösterimi
- 3D hologram modeli (otomatik döndürme)
- BoneSocket ile otomatik bağlantı

### S1-08: Feedback Sistemi ✅
- Haptic feedback (kazı: 0.3, başarı: 0.8)
- Audio feedback (spatial audio)
- Partikül efektleri
- Singleton pattern

## Ana Özellikler

### VR Etkileşimi
- ✅ XR Origin ile VR kontrolcü desteği
- ✅ Grab system ile nesne tutma
- ✅ Socket system ile nesne yerleştirme
- ✅ Trigger/Activate events ile kontrol

### Kazı Sistemi
- ✅ Shovel ile toprak kazma
- ✅ Hız kontrolü (yavaş kazı mümkün değil)
- ✅ Cooldown sistemi (spam engelleme)
- ✅ Partikül efektleri (kahverengi toprak)
- ✅ Log mesajları ("Kazı yapıldı!")

### Kemik Toplama Sistemi
- ✅ DinoBone prefabs (tag: "DinoBone")
- ✅ BoneSocket ile yerleştirme
- ✅ Event sistemi ile bildirim
- ✅ Otomatik tanıma (bone name → dino info)

### Tablet Sistemi
- ✅ World Space Canvas
- ✅ Dinozor bilgileri (İsim, Dönem, Açıklama, Ağırlık, Boy, Uzunluk)
- ✅ 3D hologram modeli (döndürme efekti)
- ✅ Otomatik gösterim (BoneSocket signal)
- ✅ Fade-in/out efektleri

### Fener Sistemi
- ✅ Spotlight (sıcak beyaz ışık)
- ✅ Trigger tuşu ile açma/kapama
- ✅ Haptic feedback
- ✅ Görsel feedback (emission color)
- ✅ Opsiyonel pil sistemi

### Feedback Sistemi
- ✅ Haptic feedback (kazı ve başarı)
- ✅ Audio feedback (spatial audio pool)
- ✅ Partikül efektleri
- ✅ Singleton pattern (global erişim)
- ✅ Cooldown kontrolü

## Event Sistemi

```csharp
// FeedbackManager
FeedbackManager.Instance.TriggerDigFeedback(position);
FeedbackManager.Instance.TriggerSuccessFeedback();
FeedbackManager.Instance.TriggerPickupFeedback();
FeedbackManager.Instance.TriggerDropFeedback();

// BoneSocketSystem
boneSocketSystem.OnBoneSocketed += (bone) => { /* ... */ };

// TabletManager
tabletManager.OnTabletShow += () => { /* ... */ };
tabletManager.OnContentUpdate += (info) => { /* ... */ };

// FlashlightController
flashlightController.OnFlashlightOn += () => { /* ... */ };
```

## Dinozor Bilgileri

| Kemik Adı | Dinozor | Dönem | Ağırlık | Boy | Uzunluk |
|-----------|---------|-------|---------|-----|---------|
| DinoBone | T-Rex | Kretase (68-66 MYA) | 7-8 Ton | 4-4.5m | 12-13m |
| StegosaurusBone | Stegosaurus | Jura (155-150 MYA) | 5-7 Ton | 4m | 9m |
| TriceratopsBone | Triceratops | Kretase (68-66 MYA) | 6-12 Ton | 3m | 9m |

## Performans Bütçeleri

### VR Performans
- **Hedef FPS**: 72+ (VR standartı)
- **Draw Calls**: < 100
- **Triangles**: < 100,000
- **Particles**: < 500

### Ses
- **Audio Sources**: Pool 5
- **Spatial Audio**: Linear falloff, 20m range
- **Volume**: 1.0 (ayarlanabilir)

### Haptics
- **Dig Intensity**: 0.3
- **Success Intensity**: 0.8
- **Dig Duration**: 0.1s
- **Success Duration**: 0.3s

## Kurulum Gereksinimleri

### Unity Editor
- Unity 2022.3.62f3 veya üzeri
- XR Interaction Toolkit 3.1.2
- URP 14.0.12
- Unity Input System 1.14.0
- OpenXR 1.14.3

### VR Donanımı
- Meta Quest 2/3
- HTC Vive
- Valve Index
- veya OpenXR uyumlu diğer VR headset'ler

### Unity Tags ve Layers
- **DinoBone** tag (kemikler için)
- **Soil** layer (toprak için, Layer 6)

## Bilinmeyen Sorunlar ve Çözümler

### XR Grab Çalışmıyor
- XR Origin'in sahnede olduğundan emin olun
- XRInteractionManager'ın çalıştığını kontrol edin
- Layer mask ayarlarını kontrol edin

### Haptics Çalışmıyor
- XR Controller'ın sahnede olduğundan emin olun
- FeedbackManager.Instance.SetActiveController() ile manuel atama deneyin

### Tablet Görüntülenmiyor
- BoneSocketSystem'in atanmış olduğundan emin olun
- OnBoneSocketed event'in tetiklendiğini kontrol edin
- Tablet canvas'ın World Space modunda olduğundan emin olun

### Partikül Efektleri Görünmüyor
- Particle System'in enabled olduğunu kontrol edin
- Renderer bileşeninin aktif olduğunu kontrol edin
- Camera'ın partikülleri görüntülediğinden emin olun

## Geliştirme Notları

### Kodlama Standartları
- Namespace: `Alperen.Scripts.*`
- Doc comments: Tüm public API'lerde zorunlu
- Event-driven architecture (coupling azaltmak için)
- Singleton pattern (FeedbackManager için)

### Asset Kuralları
- Tüm asset'ler `Assets/alperen/` altında
- Başka çalışanların dosyalarına dokunulmaması (kural)
- Prefab'ler `Prefabs/` altında kategorize edilmiş
- Script'ler `Scripts/` altında namespace ile organize edilmiş

### Unity Ayarları
- Color Space: Linear
- Auto Graphics API: Off
- Scripting Backend: IL2CPP
- API Compatibility Level: .NET Standard 2.1

## Test Checklist

### Basic Functionality
- [ ] XR Origin çalışıyor
- [ ] Shovel tutulabiliyor
- [ ] Flashlight tutulabiliyor ve açılıyor
- [ ] DinoBone tutulabiliyor
- [ ] BoneSocket'e kemik yerleştirilebiliyor

### Feedback
- [ ] Kazı haptic feedback çalışıyor
- [ ] Başarı haptic feedback çalışıyor
- [ ] Kazı sesi çalıyor
- [ ] Başarı sesi çalıyor
- [ ] Partikül efektleri görüntüleniyor

### Tablet
- [ ] Kemik sokulduğunda tablet görünüyor
- [ ] Dinozor bilgileri doğru gösteriliyor
- [ ] Hologram modeli döndürülüyor
- [ ] Tableti gizleyip gösterebiliyorsunuz

### Atmosphere
- [ ] Sahne karanlık
- [ ] Sis efekti görünüyor
- [ ] Ambient ışıklar çalışıyor
- [ ] Flashlight etkili çalışıyor

## Sonraki Adımlar

### Nice to Have
- [ ] S1-14: Kamera sistemi (kemik fotoğrafı çekme)
- [ ] S1-15: Tamamlama kontrol noktası (tüm kemikler bulundu)
- [ ] Daha fazla dinozor türü
- [ ] Gelişmiş mağara geometrisi
- [ ] Ses dosyaları ekleme
- [ ] Texture'lar ekleme

### Potansiyel Özellikler
- Çoklu kazı alanları
- Puan sistemi
- Başarı rozetleri
- Ses kayıtları (realistic dig sounds)
- Mağara navigasyonu
- Çok oyunculu modu

## İletişim ve Destek
- Proje Alperen namespace altında organize edilmiş
- Tüm script'ler Türkçe doc comment'leri içerir
- Event-driven architecture ile kolay genişletilebilir
