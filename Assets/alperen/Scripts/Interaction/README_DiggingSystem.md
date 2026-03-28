# DiggingSystem Kurulumu

## Dosyalar
- `Assets/alperen/Prefabs/Tools/Shovel.prefab` - Kürek prefab
- `Assets/alperen/Scripts/Interaction/DiggingSystem.cs` - Kazı sistemi scripti

## Kurulum Adımları

### 1. Soil Layer Oluştur
1. Unity Editor'da `Edit > Project Settings > Tags and Layers` açın
2. Layer 6'ya "Soil" adını verin
3. Toprak oyun nesnelerini bu layer'a atayın

### 2. Shovel Prefab'ı Sahneye Yerleştir
1. `Assets/alperen/Prefabs/Tools/Shovel.prefab` sahneye sürükleyin
2. XR Origin'in yanına yerleştirin

### 3. DiggingSystem'i Shovel'a Ekle
1. Shovel prefab seçin
2. Inspector'da "Add Component" butonuna tıklayın
3. `DiggingSystem` arayın ve ekleyin
4. Script ayarlarını yapın:
   - **Soil Layer**: 6 (Soil layer)
   - **Min Dig Velocity**: 0.5 (kazı için gerekli minimum hız)
   - **Dig Cooldown**: 0.2 (kazı arası bekleme süresi)

### 4. XR Grabbable Kontrol
Shovel prefab zaten `XRGrabInteractable` ile hazır:
- Ellerle tutulabilir
- Fırlatıldığında fiziksel düşer
- Tek el veya çift el ile kullanılabilir

## Özellikler
- ✅ Toprak ile temasda "Kazı yapıldı" logu atar
- ✅ Temas noktasında toprak parçacıkları (particle effect) oluşturur
- ✅ Hız kontrolü (yavaş hareketlerle kazı yapamazsınız)
- ✅ Cooldown sistemi (spam kazıyı önler)
- ✅ Ses desteği (ses dosyası atanabilir)
- ✅ Event sistemi (`OnDig` event'i ile diğer sistemlere bağlanabilir)

## Test Etme
1. Play mod başlat
2. VR cihazınızı bağlayın
3. Shovel'ı alın
4. Soil layer'da bir nesneye hızlıca temas edin
5. Konsolda "Kazı yapıldı" mesajını görmelisiniz
6. Temas noktasında kahverengi toprak parçacıkları görmelisiniz
