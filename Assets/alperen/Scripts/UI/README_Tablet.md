# InfoTablet ve TabletManager Sistemi

## Dosyalar
- `Assets/alperen/Prefabs/UI/InfoTablet.prefab` - Tablet prefab (Canvas UI)
- `Assets/alperen/Scripts/UI/TabletManager.cs` - Tablet yöneticisi
- `Assets/alperen/Scripts/UI/HologramController.cs` - Hologram efekti kontrolcüsü

## Kurulum Adımları

### 1. InfoTablet Prefab'ını Sahneye Ekle
1. `InfoTablet.prefab` sahneye sürükleyin
2. XR Origin'in yanına yerleştirin

### 2. TabletManager Component'ini Ekle
1. InfoTablet prefab seçin
2. "Add Component" butonuna tıklayın
3. `TabletManager` arayın ve ekleyin
4. Inspector'da ayarları yapılandırın:
   - **Tablet Canvas**: Tablet canvas objesi atanmalı
   - **Content Panel**: İçerik paneli atanmalı
   - **Info Text**: TMP_Text component'i atanmalı
   - **Hologram Container**: Hologram konteyneri atanmalı
   - **Hologram Model**: Döndürülecek model atanmalı
   - **Bone Socket System**: BoneSocketSystem component'i atanmalı

### 3. HologramController Component'ini Ekle
1. Hologram model objesini seçin
2. `HologramController` component'ini ekleyin
3. Ayarları yapılandırın:
   - **Auto Rotate**: true (otomatik döndürme)
   - **Rotation Speed**: 30 (döndürme hızı)
   - **Use Hologram Effect**: true (hologram efekti)

### 4. BoneSocketSystem ile Bağlantı
BoneSocketSystem otomatik olarak bulunur veya atanabilir:
- BoneSocketSystem'den `OnBoneSocketed` event'i
- TabletManager'de `OnBoneSocketed` metoduna bağlanır
- Kemik yuvaya girdiğinde tablet görünür olur

## TabletManager.cs Özellikleri

### Temel İşlevler
- ✅ BoneSocket'den sinyal alarak tableti görürür
- ✅ Dinozor bilgilerini doldurur (İsim, Dönem, Açıklama, Ağırlık, Boy, Uzunluk)
- ✅ 3D hologram modeli döndürür
- ✅ Görsel fade-in/out efekti
- ✅ Event sistemi

### Dinozor Bilgi Sistemi
```csharp
// Dinozor info sınıfı:
public class DinosaurInfo
{
    public string name;
    public string period;
    public string description;
    public string weight;
    public string height;
    public string length;
}
```

### API
```csharp
// Tablet'i göster/gizle:
tabletManager.ShowTablet();
tabletManager.HideTablet();
tabletManager.ToggleTablet();

// Durum kontrolü:
bool isVisible = tabletManager.IsVisible;

// Manuel dinozor bilgisi ayarla:
var info = new DinosaurInfo {
    name = "T-Rex",
    period = "Kretase",
    description = "En büyük etçil...",
    weight = "7 Ton",
    height = "4 Metre",
    length = "12 Metre"
};
tabletManager.SetDinosaurInfo(info);

// Event'lere abone olma:
tabletManager.OnTabletShow += () => Debug.Log("Tablet görüldü!");
tabletManager.OnTabletHide += () => Debug.Log("Tablet gizlendi!");
tabletManager.OnContentUpdate += (info) => Debug.Log($"İçerik güncellendi: {info.name}");
```

### Otomatik Dinozor Tanıma
Bone ismine göre otomatik bilgi:
- `DinoBone` → T-Rex
- `StegosaurusBone` → Stegosaurus
- `TriceratopsBone` → Triceratops
- Diğerleri → "Bilinmeyen Dinozor"

## HologramController.cs Özellikleri

### Hologram Efekti
- ✅ Otomatik döndürme (Auto Rotate)
- ✅ Hologram renk efekti (mavi parlama)
- ✅ Titreme efekti (Flicker)
- ✅ Nabız ölçek efekti (Pulse Scale)
- ✅ Parçacık efekti (Particles)

### API
```csharp
// Döndürme hızını ayarla:
hologramController.SetRotationSpeed(50f);

// Hologram rengini ayarla:
hologramController.SetHologramColor(Color.red);

// Hologram efektini aç/kapa:
hologramController.SetHologramEffect(true);
hologramController.SetHologramEffect(false);

// Orijinal materyalleri geri yükle:
hologramController.RestoreOriginalMaterials();

// Döndürme açısını al:
float angle = hologramController.CurrentRotationAngle;
```

## InfoTablet Prefap Yapısı

```
InfoTablet (Root)
├── TabletCanvas (UI)
│   ├── ContentPanel (Background)
│   └── InfoText (TMP_Text - İçerik)
└── HologramContainer (3D Model)
    └── HologramBase
        └── HologramModel (Döndürülen model)
```

### UI Elementleri
- **Content Panel**: Koyu mavi arka plan (RGBA: 0.1, 0.15, 0.2, 0.95)
- **Info Text**: Dinozor bilgileri gösteren TextMeshPro text
  - Font: Liberation Sans
  - Size: 28
  - Color: Beyaz
  - Rich Text: Destekli

### Hologram Elementleri
- **HologramContainer**: Tablet üstündeki 3D alan
- **HologramModel**: Döndürülen dinozor modeli
- **HologramController**: Döndürme ve efekt kontrolcüsü

## Test Etme

### Tablet Testi
1. Play mod başlat
2. VR cihazınızı bağlayın
3. DinoBone prefab'ını sahneye koyun
4. DinoBone'ı BoneSocket'in içine yerleştirin
5. Tablet otomatik görüntülenmeli
6. Dinozor bilgisi görüntülenmeli
7. Hologram modeli dönmeli
8. Tablet'i gizleyip gösterebilirsiniz

### Konsol Çıktısı
```
TabletManager: Kemik bilgisi gösteriliyor - Tyrannosaurus Rex
BoneSocketSystem: Kemik yerleştirildi! DinoBone(Clone) sokete girdi.
```

## Önerilen Ayarlar

### TabletManager
- Rotate Hologram: true
- Rotation Speed: 30f
- Hologram Scale: 1f
- Show Tablet On Start: false
- Fade Speed: 0.5f

### HologramController
- Auto Rotate: true
- Rotation Speed: 30f
- Use Hologram Effect: true
- Hologram Color: (0.5, 0.8, 1, 0.6) - Mavi ton
- Pulse Scale: false (veya true için Pulse Speed: 1f)

## Notlar
- BoneSocketSystem otomatik bulunur (FindObjectOfType)
- Canvas World Space modunda çalışır (RenderMode: 2)
- TextMeshPro kullanılır (rich text destekli)
- Hologram materyali otomatik oluşturulur
- Tablet el ile de gösterilebilir (ShowTablet/HideTablet)
- Dinozor bilgileri genişletilebilir (DinosaurInfo sınıfı)

## Event Bağlantısı
Inspector'dan UnityEvent ile:
```
BoneSocketSystem (On Bone Socketed)
    └── TabletManager (ShowTablet)
```

Kod ile:
```csharp
boneSocketSystem.OnBoneSocketed += (bone) => {
    tabletManager.ShowTablet();
    var info = tabletManager.GetDinosaurInfo(bone.name);
    tabletManager.UpdateContent(info);
};
```
