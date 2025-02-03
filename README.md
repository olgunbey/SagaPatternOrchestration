# Saga Orchestration
Servisler arasındaki veri tutarlılığını State Machine ile sağlıyoruz.
Burada ilk başta transaction Order'dan başlar. Bu esnada bir OrderStarted tetikleyici event fırlatılır.
- State Machine bu tetikleyicinin ardından tanımlanan eventleri yönetir.
- Bu kurguda Ödeme alındıktan sonra stokta azalma olacaktır.
- Ödeme başarılı olur ise Order Servisine ve Stock Servisine event Publish edilecektir.
- Eğer Ödeme başarısız olur ise sadece Order Servisine haber verilir.
