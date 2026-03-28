# VR Arkeolojik Kazı Projesi - Kurulum Rehberi

## Hızlı Başlangıç (5 Dakika)

### 1. Unity'de Proje Açın
```
Unity Editor'de "Open" tıklayın
SanalMuzeGezi klasörünü seçin
Open tıklayın
```

### 2. Sahneyi Açın
```
Assets/alperen/Scenes/ExcavationCave.unity
Çift tıklayarak açın
```

### 3. Prefab'ları Yerleştirin
Sahneye sürükleyin:
- `Assets/alperen/Prefabs/Tools/XROrigin.prefab`
- `Assets/alperen/Prefabs/Tools/Shovel.prefab`
- `Assets/alperen/Prefabs/Tools/Flashlight.prefab`
- `Assets/alperen/Prefabs/Environment/BoneSocket.prefab`
- `Assets/alperen/Prefabs/UI/InfoTablet.prefab`
- `Assets/alperen/Prefabs/Collectibles/DinoBone.prefab` (2-3 tane)

### 4. Scripts Ekleyin

#### Shovel Prefab
1. Shovel prefab seçin
2. "Add Component" → "DiggingSystem"
3. Inspector'da ayarları kontrol edin:
   - Soil Layer: 6 (veya "Soil")
   - Min Dig Velocity: 0.5
   - Dig Cooldown: 0.2

#### BoneSocket Prefab
1. BoneSocket prefab seçin
2. "Add Component" → "BoneSocketSystem"
3. Inspector'da ayarları kontrol edin:
   - Accepted Tag: DinoBone
   - Socket Visual: Soket görselini atayın
   - On Bone Socketed: UnityEvent'leri tanımlayın

#### Flashlight Prefab
1. Flashlight prefab seçin
2. "Add Component" → "FlashlightController"
3. Inspector'da ayarları kontrol edin:
   - Spotlight: Fener ucundaki ışık atanmalı
   - Flashlight Glow: Glow efekti atanabilir
   - Toggle Sounds: Ses dosyaları atanabilir

#### Tablet Prefab
1. InfoTablet prefab seçin
2. "Add Component" → "TabletManager"
3. Inspector'da ayarları kontrol edin:
   - Tablet Canvas: Canvas atanmalı
   - Content Panel: Panel atanmalı
   - Info Text: TMP_Text atanmalı
   - Hologram Container: 3D container atanmalı
   - Hologram Model: Döndürülecek model atanmalı

#### Hologram Model
1. HologramModel seçin (InfoTablet içinde)
2. "Add Component" → "HologramController"
3. Inspector'da ayarları kontrol edin:
   - Auto Rotate: true
   - Rotation Speed: 30
   - Use Hologram Effect: true

### 5. FeedbackManager Kurulumu

1. Sahneye boş bir GameObject ekleyin: "FeedbackManager"
2. FeedbackManager.cs component'ini ekleyin
3. Inspector'da:
   - Dig Haptic Intensity: 0.3
   - Success Haptic Intensity: 0.8
   - Dig Sound: Ses dosyası atanabilir
   - Success Sound: Ses dosyası atanabilir
   - Soil Particle Prefab: SoilParticles.prefab

### 6. AtmosphereManager Kurulumu

1. Sahneye boş bir GameObject ekleyin: "AtmosphereManager"
2. AtmosphereManager.cs component'ini ekleyin
3. Inspector'da:
   - Ambient Intensity: 0.05
   - Use Fog: true
   - Fog Density: 0.05
   - Point Light Count: 3

### 7. Play Mod Başlatın
```
Play butonuna tıklayın
VR headset'inizi bağlayın
Test etmeye başlayın!
```

## Detaylı Kurulum

### Unity Tags Oluşturma

1. `Edit > Project Settings > Tags and Layers` açın
2. Tags kısmında "+" tıklayın
3. "DinoBone" yazıp Save yapın

### Unity Layers Oluşturma

1. `Edit > Project Settings > Tags and Layers` açın
2. Layers kısmında Layer 6'ya tıklayın
3. "Soil" yazın

### XR Origin Ayarları

1. XROrigin prefab seçin
2. Inspector'da XR Origin component'ini kontrol edin:
   - Camera Floor Offset Object: Camera Offset atanmalı
   - Camera: Main Camera atanmalı
3. Eller için Direct Interactor ekleyin (otomatik gelmeli)

### Shovel Collider Ayarları

1. Shovel prefab içinde ShovelHead seçin
2. BoxCollider kontrol edin:
   - Is Trigger: false
   - Size: Kürek boyutuna göre ayarla

### Soil Nesnelerini Layer'a Atama

1. Toprak nesnelerini sahnede oluşturun
2. Inspector'da Layer: "Soil" seçin
3. MeshRenderer ve Collider ekleyin

### Event Bağlantıları

#### DiggingSystem → FeedbackManager
```csharp
// DiggingSystem.cs PerformDig() fonksiyonunda:
FeedbackManager.Instance.TriggerDigFeedback(contactPoint);
```

#### BoneSocketSystem → FeedbackManager + TabletManager
```csharp
// BoneSocketSystem.cs OnBoneSocketEnter() fonksiyonunda:
FeedbackManager.Instance.TriggerSuccessFeedback();
TabletManager.Instance.ShowTablet();
```

## Hata Giderme

### Hata: "XR Origin not found"
**Çözüm**:
1. XROrigin prefab'ı sahneye ekleyin
2. XR Interaction Manager'in sahnede olduğundan emin olun
3. XR Settings'de XR General Settings atayın

### Hata: "DiggingSystem not triggering"
**Çözüm**:
1. Shovel'ın Rigidbody olduğu için emin olun
2. Collider'ın Is Trigger = false olduğundan emin olun
3. Toprak nesnelerinin "Soil" layer'ında olduğundan emin olun
4. Min Dig Velocity'ı düşürün (0.3 gibi)

### Hata: "Tablet not showing"
**Çözüm**:
1. TabletManager'ın BoneSocketSystem'e bağlı olduğundan emin olun
2. OnBoneSocketed event'in tetiklendiğini log'dan kontrol edin
3. Tablet Canvas'ın enabled olduğundan emin olun

### Hata: "Haptics not working"
**Çözüm**:
1. FeedbackManager.Instance.SetActiveController() çağırın
2. XR Controller'ın sahnede olduğundan emin olun
3. Haptic intensity değerlerini artırın

### Hata: "Particles not visible"
**Çözüm**:
1. Particle System'in enabled olduğundan emin olun
2. Particle Renderer'ın aktif olduğundan emin olun
3. Camera'ın Layer Mask'ında particle layer'ın olduğundan emin olun

## Ses Dosyaları Ekleme

### Ses Dosyalarını İçe Aktarma
```
Assets/alperen/Audio/ klasörüne ses dosyalarını kopyalayın:
- Dig_Sound.wav (kazı sesi)
- Success_Sound.wav (başarı sesi)
- Pickup_Sound.wav (alma sesi)
- Drop_Sound.wav (bırakma sesi)
```

### FeedbackManager'a Ses Atama
1. FeedbackManager prefab seçin
2. Inspector'da:
   - Dig Sound: Dig_Sound.wav sürükleyin
   - Success Sound: Success_Sound.wav sürükleyin
   - Pickup Sound: Pickup_Sound.wav sürükleyin
   - Drop Sound: Drop_Sound.wav sürükleyin
   - Volume: 1.0

## Texture'lar Ekleme

### Texture'ları İçe Aktarma
```
Assets/alperen/Textures/ klasörüne texture'ları kopyalayın:
- Soil_Albedo.png
- Bone_Albedo.png
- CaveWall_Albedo.png
```

### Texture'ları Prefab'lara Atama
1. Prefab seçin
2. Inspector'da Material'ı seçin
3. Albedo texture'ını sürükleyin

## Build Ayarları

### Android (Quest 2/3) için Build
```
File > Build Settings
Platform: Android
Switch Platform tıklayın
Player Settings:
  - Package Name: com.alperen.vrexcavation
  - Minimum API Level: 29 (Android 10)
  - Target API Level: 33 (Android 13)
  - Graphics APIs: OpenGLES3, Vulkan
XR Settings:
  - XR Plug-in Management: OpenXR
Build tıklayın
```

### Windows (PCVR) için Build
```
File > Build Settings
Platform: Windows, Mac, Linux (Standalone)
Target Platform: Windows
Architecture: x86_64
Player Settings:
  - Color Space: Linear
  - Auto Graphics API: Off
  - Graphics APIs: Direct3D11
XR Settings:
  - XR Plug-in Management: OpenXR
Build tıklayın
```

## Test Adımları

### 1. Basic Interaction Test
1. Play mod başlat
2. VR headset bağlayın
3. XROrigin kontrol edin
4. Eller hareket ediyor mu?
5. Shovel tutulabiliyor mu?
6. Flashlight tutulabiliyor mu?

### 2. Digging Test
1. Shovel alın
2. Toprak nesnesine hızlıca dokunun
3. Haptic feedback hissediniz mi?
4. Kazı sesi çalıyor mu?
5. Partikül efektleri görüyor musunuz?

### 3. Bone Socket Test
1. DinoBone alın
2. BoneSocket'e yerleştirin
3. Bağarı haptic feedback hissediniz mi?
4. Tableti görüyor musunuz?
5. Dinozor bilgileri görüntüleniyor mu?

### 4. Tablet Test
1. Tablet açıkken hologram görüyor musunuz?
2. Hologram döndürülüyor mu?
3. Tableti gizleyip gösterebiliyor musunuz?

### 5. Flashlight Test
1. Flashlight alın
2. Trigger tuşuna basın
3. Fener açılıyor mu?
4. Haptic feedback hissediniz mi?
5. Işık mağarayı aydınlatıyor mu?

### 6. Atmosphere Test
1. Sahne karanlık mı?
2. Sis efekti görüyor musunuz?
3. Ambient ışıklar çalışıyor mu?
4. Flashlight ile iyi görünüyor mu?

## Performans Optimizasyonu

### Draw Call Azaltma
- Particle system'leri birleştirin
- Material'ları paylaşın
- LOD (Level of Detail) kullanın

### Triangle Count Azaltma
- Modelleri düşük poligon yapın
- Simplify tool kullanın
- LOD grupları oluşturun

### Particle Optimizasyonu
- Particle count azaltın (15 → 10)
- Lifetime kısaltın (2s → 1.5s)
- Renderer mode'ları kontrol edin

### Audio Optimizasyonu
- Audio sources'i kullanmayın yok edin (pool kullanın)
- Spatial audio range ayarlayın
- Compressed ses formatları kullanın

## Güvenli Geliştirme

### Git Kullanımı
```
git status                        # Değişiklikleri kontrol et
git add .                        # Tüm değişiklikleri stage'e al
git commit -m "feature: X eklendi"  # Commit yap
git push                          # Remote'a gönder
```

### Versiyon Kontrolü
- Asset'leri düzenli olarak commit edin
- Prefab'ler ve scene'ler versionlanmalı
- Script'ler doc comment'leri içermeli

### Backup
- Düzenli olarak project klasörünü backup alın
- Unity version kontrol edin (compatibility için)
- Asset database'i temizlemek için "Assets > Reimport All"

## Sonraki Adımlar

### Öğrenmek İçin
- XR Interaction Toolkit dokümantasyonu
- Unity VR geliştirme rehberleri
- Particle system optimizasyonu
- Spatial audio teknikleri

### Geliştirmek İçin
- Daha fazla dinozor türü ekleme
- Mağara tasarımını geliştirme
- Ses kayıtları ekleme
- Texture'lar ve materyaller ekleme
- UI tasarımını geliştirme

### Deploy İçin
- Quest store için hazırlık
- PCVR platformları için optimize
- Multiplayer desteği
- Save/Load sistemi

## İletişim ve Destek

### Sorun Raporlama
- Console log'larını paylaşın
- Unity version bilgisini ekleyin
- VR headset modelini belirtin
- Adım adım tekrarlanabilirlik sağlayın

### Feature İstekleri
- İstediği özelliği açıklayın
- Neden gerekli olduğunu belirtin
- Alternatif çözümler önerin
- Öncelik belirtin

## Ek Kaynaklar

### Unity Dokümantasyon
- https://docs.unity3d.com/
- https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.1/

### VR Geliştirme
- https://learn.unity.com/
- https://developer.oculus.com/documentation/
- https://valvesoftware.github.io/steamvr/

### Audio
- https://freesound.org/ (ücretsiz sesler)
- https://www.purple-planet.com/ (ücretsiz müzik)

### Partikül Efektleri
- https://u3d.as/content/ (Unity Asset Store)
- https://assetstore.unity.com/

## Başarılar!
VR arkeolojik kazı projeniz için hazır. İyi eğlenceler!
