using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaliyetlendirmeSistemi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-KIMUOA0\SQLEXPRESS;Initial Catalog=DbMaliyetlendirmeSistemi;Integrated Security=True");


        void MalzemeListe()
        {
            using (SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-KIMUOA0\SQLEXPRESS;Initial Catalog=DbMaliyetlendirmeSistemi;Integrated Security=True"))
            {
                try
                {
                    connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter("select * from TBLMALZEME", connection);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }
        void UrunListe()
        {
            try
            {
                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();
                }
                SqlDataAdapter da2 = new SqlDataAdapter("select * from TBLURUN", baglanti);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);
                dataGridView1.DataSource = dt2;
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close();
                }
            }

        }
        void Kasa()
        {
            SqlDataAdapter da3 = new SqlDataAdapter("select * from TBLKASA", baglanti);
            DataTable dt3 = new DataTable();
            da3.Fill(dt3);
            dataGridView1.DataSource = dt3;

        }

        void Urunler()
        {
            try
            {
                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();
                }

                SqlDataAdapter da = new SqlDataAdapter("select * from TBLURUN", baglanti);
                DataTable dt = new DataTable();
                da.Fill(dt);

                comboBoxURUN.ValueMember = "URUNID";
                comboBoxURUN.DisplayMember = "AD";
                comboBoxURUN.DataSource = dt;
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close();
                }
            }

        }

        void Malzeme()
        {
            try
            {
                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();
                }
                SqlDataAdapter da = new SqlDataAdapter("select * from TBLMALZEME", baglanti);
                DataTable dt = new DataTable();
                da.Fill(dt);
                comboBoxMALZEME.ValueMember = "MALZEMEID";
                comboBoxMALZEME.DisplayMember = "AD";
                comboBoxMALZEME.DataSource = dt;
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close();
                }
            }
        }

        private void UrunSatis(double sFiyat, double mFiyat)
        {
            try
            {
                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();
                }

                // Satıştan elde edilen gelir kasa girişine eklenir
                SqlCommand girisKomut = new SqlCommand("UPDATE TBLKASA SET GIRIS = ISNULL(GIRIS, 0) + @giris WHERE EXISTS (SELECT 1 FROM TBLKASA)", baglanti);
                girisKomut.Parameters.AddWithValue("@giris", sFiyat);
                girisKomut.ExecuteNonQuery();

                // Ürünün üretim maliyeti kasa çıkışına eklenir
                SqlCommand cikisKomut = new SqlCommand("UPDATE TBLKASA SET CIKIS = ISNULL(CIKIS, 0) + @cikis WHERE EXISTS (SELECT 1 FROM TBLKASA)", baglanti);
                cikisKomut.Parameters.AddWithValue("@cikis", mFiyat);
                cikisKomut.ExecuteNonQuery();

                MessageBox.Show("Ürün satışı ve kasa işlemleri başarıyla güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close();
                }
            }
        }

        private void MalzemeAlimi(double malzemeFiyat)
        {
            try
            {
                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();
                }

                // Malzeme alımı kasa çıkışına eklenir
                SqlCommand komut = new SqlCommand("UPDATE TBLKASA SET CIKIS = ISNULL(CIKIS, 0) + @cikis WHERE EXISTS (SELECT 1 FROM TBLKASA)", baglanti);
                komut.Parameters.AddWithValue("@cikis", malzemeFiyat);
                komut.ExecuteNonQuery();

                MessageBox.Show("Malzeme alımı başarılı bir şekilde kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close();
                }
            }
        }
        private void KasaBakiyeHesapla()
        {
            try
            {
                // Ensure the connection is open
                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();
                }

                // SQL command to calculate the balance
                SqlCommand komut = new SqlCommand("SELECT ISNULL(SUM(GIRIS), 0) - ISNULL(SUM(CIKIS), 0) AS BAKIYE FROM TBLKASA", baglanti);

                // Execute the command and get the result
                object result = komut.ExecuteScalar();

                // Set the result to the text box
                if (result != null)
                {
                    textBoxKasaBakiye.Text = result.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Ensure the connection is closed
                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close();
                }
            }
        }





        private void Form1_Load(object sender, EventArgs e)
        {
            MalzemeListe();
            Urunler();
            Malzeme();
        }

        private void buttonMalzemeListe_Click(object sender, EventArgs e)
        {
            MalzemeListe();
        }

        private void buttonUrunListe_Click(object sender, EventArgs e)
        {
            UrunListe();
        }

        private void buttonKasa_Click(object sender, EventArgs e)
        {
            Kasa();
        }

        private void buttonMALZEMEEKLE_Click(object sender, EventArgs e)
        {
            try
            {
                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();
                }

                SqlCommand komut = new SqlCommand("insert into TBLMALZEME (AD,STOK,FIYAT,NOTLAR) values (@p1,@p2,@p3,@p4)", baglanti);
                komut.Parameters.AddWithValue("@p1", textBoxMALZEMEAD.Text);
                komut.Parameters.AddWithValue("@p2", decimal.Parse(textBoxMALZEMESTOK.Text));
                komut.Parameters.AddWithValue("@p3", decimal.Parse(textBoxMALZEMEFIYAT.Text));
                komut.Parameters.AddWithValue("@p4", textBoxMALZEMENOTLAR.Text);
                komut.ExecuteNonQuery();

                MessageBox.Show("Malzeme başarılı bir şekilde eklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Malzeme listelemesi ve comboBoxMALZEME güncellemesi
                MalzemeListe();   // Datagrid güncellemesi
                Malzeme();        // ComboBox güncellemesi
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close();
                }
            }
        }

        private void buttonMAZLEMEGETIR_Click(object sender, EventArgs e)
        {
            int secilen = dataGridView1.SelectedCells[0].RowIndex;

            textBoxMALZEMEAD.Text = dataGridView1.Rows[secilen].Cells[1].Value.ToString();
            textBoxMALZEMESTOK.Text = dataGridView1.Rows[secilen].Cells[2].Value.ToString();
            textBoxMALZEMEFIYAT.Text = dataGridView1.Rows[secilen].Cells[3].Value.ToString();
            textBoxMALZEMENOTLAR.Text = dataGridView1.Rows[secilen].Cells[4].Value.ToString();
        }

        private void buttonURUNEKLE_Click(object sender, EventArgs e)
        {
            try
            {
                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();
                }

                SqlCommand komut = new SqlCommand("INSERT INTO TBLURUN (AD) VALUES (@p1)", baglanti);
                komut.Parameters.AddWithValue("@p1", textBoxURUNAD.Text);
                komut.ExecuteNonQuery();

                MessageBox.Show("Ürün başarılı bir şekilde eklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Ürünler metodunu çağırarak ComboBox'ı güncelliyoruz.
                Urunler();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close();
                }
            }
        }

        private void buttonEKLE1_Click(object sender, EventArgs e)
        {
            try
            {
                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();
                }

                SqlCommand komut = new SqlCommand("INSERT INTO TBLFIRIN (URUNID, MALZEMEID, MIKTAR, MALIYET) VALUES (@p1, @p2, @p3, @p4)", baglanti);
                komut.Parameters.AddWithValue("@p1", comboBoxURUN.SelectedValue);
                komut.Parameters.AddWithValue("@p2", comboBoxMALZEME.SelectedValue);
                komut.Parameters.AddWithValue("@p3", decimal.Parse(textBoxMIKTAR.Text));
                komut.Parameters.AddWithValue("@p4", decimal.Parse(textBoxMALIYET.Text));
                komut.ExecuteNonQuery();

                MessageBox.Show("Malzeme başarılı bir şekilde eklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                listBox1.Items.Add(comboBoxMALZEME.Text + " - " + textBoxMALIYET.Text);
                UrunListe();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close();
                }
            }

        }

        private void buttonURUNGUNCELLE_Click(object sender, EventArgs e)
        {
            // Giriş verilerini kontrol et
            if (string.IsNullOrWhiteSpace(textBoxURUNMFIYAT.Text) ||
                string.IsNullOrWhiteSpace(textBoxURUNSFIYAT.Text) ||
                string.IsNullOrWhiteSpace(textBoxURUNSTOK.Text) ||
                string.IsNullOrWhiteSpace(textBoxURUNID.Text))
            {
                MessageBox.Show("Lütfen tüm alanları doldurduğunuzdan emin olun.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Parse işlemi için değişkenler
            decimal maliyetFiyat, satisFiyat;
            int stok, urunId;

            // Girişlerin uygun formatta olup olmadığını kontrol et
            if (!decimal.TryParse(textBoxURUNMFIYAT.Text, out maliyetFiyat))
            {
                MessageBox.Show("Geçerli bir maliyet fiyatı girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!decimal.TryParse(textBoxURUNSFIYAT.Text, out satisFiyat))
            {
                MessageBox.Show("Geçerli bir satış fiyatı girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!int.TryParse(textBoxURUNSTOK.Text, out stok))
            {
                MessageBox.Show("Geçerli bir stok değeri girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!int.TryParse(textBoxURUNID.Text, out urunId))
            {
                MessageBox.Show("Geçerli bir ürün ID'si girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Veritabanı bağlantısını açıyoruz.
                baglanti.Open();

                // SQL güncelleme sorgusunu oluşturuyoruz.
                SqlCommand komut = new SqlCommand("update TBLURUN set MFIYAT=@p1, SFIYAT=@p2, STOK=@p3 where URUNID=@p4", baglanti);

                // Parametrelerle değerleri bağlıyoruz.
                komut.Parameters.AddWithValue("@p1", maliyetFiyat);  // Maliyet Fiyatı
                komut.Parameters.AddWithValue("@p2", satisFiyat);    // Satış Fiyatı
                komut.Parameters.AddWithValue("@p3", stok);          // Stok
                komut.Parameters.AddWithValue("@p4", urunId);        // Ürün ID'si

                // Sorguyu çalıştırıyoruz.
                komut.ExecuteNonQuery();

                // Bilgi mesajı gösteriyoruz.
                MessageBox.Show("Ürün fiyatı ve stoğu başarılı bir şekilde güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Bağlantıyı kapatıyoruz.
                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close();
                }
            }

            // Ürün listesini yeniliyoruz.
            UrunListe();
        }


        private void textBoxMIKTAR_TextChanged(object sender, EventArgs e)
        {
            // Miktarın doğru bir şekilde girilip girilmediğini kontrol ediyoruz
            if (double.TryParse(textBoxMIKTAR.Text, out double miktar) && miktar > 0)
            {
                // Malzeme seçildi mi kontrolü yapılıyor
                if (comboBoxMALZEME.SelectedValue != null)
                {
                    try
                    {
                        // Bağlantı açılıyor
                        if (baglanti.State == ConnectionState.Closed)
                        {
                            baglanti.Open();
                        }

                        // Malzeme fiyatını sorgulamak için SQL komutu oluşturuluyor
                        SqlCommand komut = new SqlCommand("SELECT FIYAT FROM TBLMALZEME WHERE MALZEMEID=@P1", baglanti);
                        komut.Parameters.AddWithValue("@P1", comboBoxMALZEME.SelectedValue);

                        // Veri okuma işlemi başlıyor
                        using (SqlDataReader dr = komut.ExecuteReader())
                        {
                            if (dr.HasRows && dr.Read())
                            {
                                // Fiyat bilgisini alıp maliyet hesaplamasını yapıyoruz
                                double fiyat = Convert.ToDouble(dr["FIYAT"]);
                                textBoxMALIYET.Text = (fiyat / 1000 * miktar).ToString("F2");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Olası hatalar burada işlenebilir
                        MessageBox.Show("Bir hata oluştu: " + ex.Message);
                    }
                    finally
                    {
                        // Bağlantı kapatılıyor
                        if (baglanti.State == ConnectionState.Open)
                        {
                            baglanti.Close();
                        }
                    }
                }
            }
            else
            {
                // Eğer miktar geçersizse maliyet sıfırlanıyor
                textBoxMALIYET.Text = "0";
            }
        }


        private void comboBoxMALZEME_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int secilen = dataGridView1.SelectedCells[0].RowIndex;

            // Hücrelerin içindeki gerçek değerleri almak için Value özelliğini kullanıyoruz.
            textBoxURUNID.Text = dataGridView1.Rows[secilen].Cells[0].Value.ToString();
            textBoxURUNAD.Text = dataGridView1.Rows[secilen].Cells[1].Value.ToString();

            baglanti.Open();

            // URUNID bir integer olduğu için, textBoxURUNID içeriğini int'e dönüştürüyoruz.
            SqlCommand komut = new SqlCommand("select sum(MALIYET) from TBLFIRIN where URUNID=@P1", baglanti);
            komut.Parameters.AddWithValue("@P1", int.Parse(textBoxURUNID.Text));

            SqlDataReader dr = komut.ExecuteReader();
            while (dr.Read())
            {
                textBoxURUNMFIYAT.Text = dr[0].ToString();
            }

            baglanti.Close();
        }

        private void buttonFIRINLIST_Click(object sender, EventArgs e)
        {
            try
            {
                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();
                }
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM TBLFIRIN", baglanti);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close();
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
