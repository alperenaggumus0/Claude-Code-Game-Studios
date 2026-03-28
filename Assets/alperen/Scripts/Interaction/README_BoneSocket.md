# BoneSocket Sistemi Kurulumu

## Dosyalar
- `Assets/alperen/Prefabs/Collectibles/DinoBone.prefab` - Dinozor kemik prefab
- `Assets/alperen/Prefabs/Environment/BoneSocket.prefab` - Kemik yuvası prefab
- `Assets/alperen/Scripts/Interaction/BoneSocketSystem.cs` - Yuva olay sistemi
- `Assets/alperen/Scripts/Interaction/DinoBoneTagFilter.cs` - Tag filtresi

## Kurulum Adımları

### 1. DinoBone Tag'ını Oluştur
1. Unity Editor'da `Edit > Project Settings > Tags and Layers` açın
2. "Tags" kısmında "+" tıklayın
3. "DinoBone" adını girip "Save" yapın

### 2. Prefab'ları Sahneye Yerleştir
1. `DinoBone.prefab` sahneye sürükleyin
2. `BoneSocket.prefab` sahneye sürükleyin

### 3. BoneSocket'i Yapılandır
1. BoneSocket prefab seçin
2. Inspector'da component'leri kontrol edin:
   - **XRSocketInteractor** - Otomatik olarak çalışır
   - **BoneSocketSystem** component'i eklenmeli:
     - `Accepted Tag`: "DinoBone"
     - `Allow Multiple Bones`: false (tek kemik için)
     - `Socket Visual`: Soket görsel nesnesi atanabilir
     - `On Bone Socketed`: UnityEvent - diğer sistemlere bağlanabilir

### 4. Event Sistemi Kullanımı
BoneSocketSystem.cs'de hazır event sistemi var:

```csharp
// Kod ile event'e abone olma:
boneSocketSystem.OnBoneSocketed += (bone) => {
    Debug.Log($"Kemik algılandı: {bone.name}");
};

// Inspector'da UnityEvent ile bağlama:
// On Bone Socketed alanında fonksiyon seçin veya parametre geçin
```

## Özellikler

### DinoBone Prefab
- ✅ "DinoBone" tag'ine sahip
- ✅ XR Grab Interactable ile tutulabilir
- ✅ Rigidbody ile fiziksel
- ✅ Kemik şeklinde basit model

### BoneSocket Prefab
- ✅ Sadece "DinoBone" tag'li objeleri kabul eder
- ✅ Kemik yuvaya girdiğinde "Kemik yerleştirildi!" logu atar
- ✅ Event sistemi (`OnBoneSocketed`)
- ✅ Görsel feedback (renk değişimi)
- ✅ Haptic feedback (kontrolör titremesi)

### BoneSocketSystem API
```csharp
// Soketin dolu olup olmadığını kontrol et
bool hasBone = boneSocket.HasBone;

// Mevcut kemik objesini al
GameObject bone = boneSocket.GetCurrentBone();

// Kemik sayısını al
int count = boneSocket.BoneCount;

// Soketi temizle
boneSocket.ClearSocket();
```

## Test Etme
1. Play mod başlat
2. VR cihazınızı bağlayın
3. DinoBone'ı alın
4. BoneSocket'in içine yerleştirin
5. Konsolda "Kemik yerleştirildi!" mesajını görmelisiniz
6. Soket rengi yeşile dönmeli (activeColor)
7. Kemik çektiğinizde gri renge dönmesi gerekir

## Notlar
- Sadece "DinoBone" tag'ine sahip objeler kabul edilir
- Event sistemi ile tablet gösterimi, ses çalma veya puan sistemi eklenebilir
- Multiple bones modu açılarak birkaç kemik tek yuvaya toplanaabilir
