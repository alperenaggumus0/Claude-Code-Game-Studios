# Flashlight ve Atmosfer Sistemi

## Dosyalar
- `Assets/alperen/Prefabs/Tools/Flashlight.prefab` - Fener prefab
- `Assets/alperen/Scripts/Interaction/FlashlightController.cs` - Fener kontrol scripti
- `Assets/alperen/Scripts/Environment/AtmosphereManager.cs` - Mağara atmosferi yöneticisi

## Kurulum Adımları

### 1. Flashlight Prefab'ı Sahneye Ekle
1. `Flashlight.prefab` sahneye sürükleyin
2. XR Origin'in yanına yerleştirin

### 2. FlashlightController Component'ini Ekle
1. Flashlight prefab seçin
2. "Add Component" butonuna tıklayın
3. `FlashlightController` arayın ve ekleyin
4. Inspector'da ayarları kontrol edin:
   - **Spotlight**: Fener ucundaki ışık atanmalı
   - **FlashlightGlow**: Fener ucundaki parlayan efekti atanabilir
   - **Toggle On/Off Sounds**: Ses dosyaları atanabilir
   - **On/Off Colors**: Görsel feedback renkleri

### 3. AtmosphereManager'ı Sahneye Ekle
1. Boş bir GameObject oluşturun: "AtmosphereManager"
2. Bu GameObject'e `AtmosphereManager.cs` component'ini ekleyin
3. Inspector'da ayarları yapılandırın:
   - **Ambient Intensity**: 0.05 (karanlık mağara)
   - **Use Fog**: true (sis efekti)
   - **Fog Density**: 0.05 (sis yoğunluğu)
   - **Point Light Count**: 3 (hafif ambient ışıklar)

## FlashlightController.cs Özellikleri

### Temel İşlevler
- ✅ Trigger tuşu (Activate event) ile açma/kapama
- ✅ Spotlight otomatik açılıp kapanır
- ✅ Görsel feedback (renk değişimi)
- ✅ Haptic feedback (kontrolör titremesi)
- ✅ Ses efektleri desteği
- ✅ Event sistemi

### Event API
```csharp
// Kod ile event'e abone olma:
flashlightController.OnFlashlightOn += () => {
    Debug.Log("Fener açıldı!");
};

flashlightController.OnFlashlightOff += () => {
    Debug.Log("Fener kapatıldı!");
};

// Feneri manuel kontrol etme:
flashlightController.SetFlashlightState(true);  // Aç
flashlightController.SetFlashlightState(false); // Kapat

// Durum kontrolü:
bool isOn = flashlightController.IsOn;
```

### Pil Sistemi (Opsiyonel)
```csharp
// Pil sistemini etkinleştir:
// Battery Drain Rate > 0 yapın (örneğin: 0.01f)

// Pil seviyesini al:
float batteryLevel = flashlightController.BatteryLevel;

// Pil değiştir:
flashlightController.ReplaceBattery();

// Manuel pil seviyesi ayarla:
flashlightController.SetBatteryLevel(0.5f); // %50
```

## AtmosphereManager.cs Özellikleri

### Atmosfer Ayarları
- ✅ Koyu mağara ortamı için ambient ışık
- ✅ Sis efekti (atmosferik derinlik)
- ✅ Hafif ambient point lights (mağara parlaması)
- ✅ Runtime'da değiştirilebilir ayarlar

### API
```csharp
// Atmosferi uygula:
atmosphereManager.ApplyAtmosphereSettings();

// Orijinal ayarlara dön:
atmosphereManager.RestoreOriginalSettings();

// Ambient ışığı değiştir (animasyon için):
atmosphereManager.LerpAmbientIntensity(0.5f, 2f); // 2 saniyede %50'ye çık

// Sis aç/kapa:
atmosphereManager.ToggleFog(true);
atmosphereManager.ToggleFog(false);

// Sis yoğunluğu ayarla:
atmosphereManager.SetFogDensity(0.1f);

// Ambient yoğunluğu ayarla:
atmosphereManager.SetAmbientIntensity(0.2f);

// Mevcut değerleri al:
float currentAmbient = atmosphereManager.CurrentAmbientIntensity;
bool fogEnabled = atmosphereManager.IsFogEnabled;
```

## Test Etme

### Flashlight Testi
1. Play mod başlat
2. VR cihazınızı bağlayın
3. Flashlight'ı alın
4. Trigger tuşuna basın → Fener açılmalı
5. Tekrar basın → Fener kapanmalı
6. Kontrolör titremesini hissetmelisiniz
7. Konsolda "Flashlight: Açıldı!" ve "Flashlight: Kapatıldı!" görmelisiniz

### Atmosphere Testi
1. Play mod başlat
2. AtmosphereManager'ın çalıştığını kontrol edin
3. Sahne karanlık olmalı (ambient ışık düşük)
4. Sis efekti olmalı (uzak nesneler bulanık)
5. Flashlight açıkken etrafları aydınlatmalı

## Önerilen Ayarlar

### Fener Işığı
- Intensity: 2.0
- Range: 15.0
- Spot Angle: 45°
- Color: 1.0, 0.95, 0.8 (sıcak beyaz)

### Mağara Atmosferi
- Ambient Intensity: 0.05
- Fog Density: 0.05
- Point Light Intensity: 0.5
- Point Light Color: 0.3, 0.2, 0.1 (turuncu-kahve ton)

## Notlar
- Flashlight'ın çalışması için XR Grab Interactable gerekli
- Trigger tuşu genellikle VR controller'ın birincil tuşudur
- AtmosphereManager otomatik olarak Start() fonksiyonunda çalışır
- VR performansı için gölgeler kapatılmıştır (LightShadows.None)
