using System;
using System.Collections.Generic;
using System.Windows;
using System.Runtime.Serialization;
using System.Net;
using System.Runtime.Serialization.Json;

namespace asd
{
    public partial class MainWindow : Window
    {
        List<DataSerie> jsonResponseData = new List<DataSerie>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.clearDataGrid();
            string start = startDate.Text;
            string end = endDate.Text;
            string url = "https://www.banxico.org.mx/SieAPIRest/service/v1/series/SF43718/datos/";
            url = url + start + "/" + end;
            this.generateData(url);
        }

        private void buildDataGrid ()
        {
            data.ItemsSource = this.jsonResponseData;
        }

        private void clearDataGrid()
        {
            List<DataSerie> aux = new List<DataSerie>();
            data.ItemsSource = aux;
            error.Content = "";
        }

        private void generateData(string url)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Accept = "application/json";
                request.Headers["Bmx-Token"] = "abe3d5c539c596485933c86a69b205d5b725e61a3c73148259b03658acd7cd1d";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Response));
                object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                Response jsonResponse = objResponse as Response;
                if (jsonResponse.seriesResponse.series[0].Data != null)
                {
                    foreach (DataSerie item in jsonResponse.seriesResponse.series[0].Data)
                    {
                        this.jsonResponseData.Add(item);
                    }
                    this.buildDataGrid();
                } else
                {
                    error.Content = "No hay registros";
                }
            }
            catch (Exception)
            {
                error.Content = "Error en la consulta, verifica el formato de fechas";
            }
            
        }
    }

    [DataContract]
    class Serie
    {
        [DataMember(Name = "titulo")]
        public string Title { get; set; }

        [DataMember(Name = "idSerie")]
        public string IdSerie { get; set; }

        [DataMember(Name = "datos")]
        public DataSerie[] Data { get; set; }

    }

    [DataContract]
    class DataSerie
    {
        [DataMember(Name = "fecha")]
        public string Date { get; set; }

        [DataMember(Name = "dato")]
        public string Data { get; set; }
    }

    [DataContract]
    class SeriesResponse
    {
        [DataMember(Name = "series")]
        public Serie[] series { get; set; }
    }

    [DataContract]
    class Response
    {
        [DataMember(Name = "bmx")]
        public SeriesResponse seriesResponse { get; set; }
    }
}
