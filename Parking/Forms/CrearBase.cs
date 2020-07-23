using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Parking.Modelos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Parking

{
    public partial class CrearBase : Form
    {
        string Serial, Conexion, Tipo;
        string rutaJson = Environment.CurrentDirectory + @"\data.json";

        public CrearBase()
        {
            InitializeComponent();
        }

        private void CrearBase_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void ComprobarServidor_Click(object sender, EventArgs e)
        {
            DatabaseExists(Servidor.Text, user.Text, pass.Text);

        }

        private void ComprobarSerial_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSerial.Text))
            {
                Serial = txtSerial.Text;
                panelServidor.BringToFront();

            }
        }
        public void DatabaseExists(string servidor, string user, string pass)
        {
            string connStr = "server = " + servidor + "; user id =" + user + "; password = " + pass + "  ; persistsecurityinfo = True";
            string connStrNueva = "server = " + servidor + "; user id =" + user + "; password = " + pass + "; database = parking_db; persistsecurityinfo = True";
            int cont = 0;
            using (MySqlConnection myConnection = new MySqlConnection(connStr))
            {
                string sql = "show databases";
                MySqlCommand myCommand = new MySqlCommand(sql, myConnection);
                try
                {
                    myConnection.Open();

                    MySqlDataReader myReader = myCommand.ExecuteReader();
                    while (myReader.Read())
                    {
                        string db = myReader["Database"].ToString();
                        if (db == "parking_db")
                        {
                            cont++;
                        }
                    }
                    myConnection.Close();

                    if (cont == 1)
                    {
                        EntityConnectionStringBuilder connectionStringBuilder = new EntityConnectionStringBuilder();
                        connectionStringBuilder.Provider = "MySql.Data.MySqlClient";
                        connectionStringBuilder.ProviderConnectionString = "server="+servidor+";user id="+user+";password="+pass+";persistsecurityinfo=True;database=parking_db";
                        connectionStringBuilder.Metadata = "res://*/EntityModel.csdl|res://*/EntityModel.ssdl|res://*/EntityModel.msl";
                        Conexion = connectionStringBuilder.ToString();


                        ModeloDatos oSettings = JsonConvert.DeserializeObject<ModeloDatos>(File.ReadAllText(rutaJson));                       
                        oSettings.CadenaConexion = Conexion;
                        oSettings.Serial = Serial;
                        oSettings.Tipo = "Enterprise";
                        var hoy = DateTime.Today.ToString();
                        oSettings.FechaInicio = hoy;
                        var termino = (DateTime.Today.AddDays(7)).ToString();
                        oSettings.FechaTermino = termino;
                        var json = JsonConvert.SerializeObject(oSettings, Formatting.None);
                        File.WriteAllText(rutaJson, json);
                        var result = MessageBox.Show("Base de datos existente", "Base de datos existente", MessageBoxButtons.OK,MessageBoxIcon.Information);
                        if (result == DialogResult.OK)
                        {
                            Process.Start(Application.ExecutablePath);
                            Application.Exit();
                        }
                    }
                    else
                    {
                        label7.Visible = true;

                        #region script base
                        string create_base = "create database parking_db; ";

                        string create_configtickets = "	CREATE TABLE Config_Tickets(	" +
                        "	ID_ticket int  primary key AUTO_INCREMENT not NULL,	" +
                        "	Incluir_Logo bit NULL,	" +
                        "	Tamaño_papel bit NULL,	" +
                        "	Estacionamiento varchar(200) null,	" +
                        "	Direccion varchar(200) null,	" +
                        "	Razon_Social varchar(100) null,	" +
                        "	Telefono varchar(10) null,	" +
                        "	RFC varchar(20) null,	" +
                        "	DatosEntrada text(2500) NULL,	" +
                        "	DatosSalida text(2500) NULL,	" +
                        "	DatosPerdido text(2500) NULL,	" +
                        "	DatosCortesia text(2500) NULL,	" +
                        "	DatosConvenio text(2500) NULL,	" +
                        "	DatosCancelado text(2500) NULL,	" +
                        "	DatosPension text(2500) NULL,	" +
                        "	Imagen longblob null,	" +
                        "	Cantidad_Copias_Entrada int not null,	" +
                        "	Cantidad_Copias_Salida int not null,	" +
                        "	Desarrollador varchar(100) null	" +
                        "	)ENGINE=InnoDB;	";

                        string create_cortes = "CREATE TABLE Cortes(	" +
                        "	Id_Corte int AUTO_INCREMENT primary key not NULL,	" +
                        "	Usuario varchar(100) NULL,	" +
                        "	FechaCorte date NULL,	" +
                        "	Importe_Parqueo decimal (10,2) NULL,	" +
                        "	Importe_Cortes decimal (10,2) NULL,	" +
                        "	Importe_Reportado decimal (10,2) NULL,	" +
                        "	StatusCorte bit NULL,	" +
                        "	CorteNormal decimal (10,2) null,	" +
                        "	CorteTarifa2 decimal (10,2) null,	" +
                        "	CortePension decimal (10,2) null,	" +
                        "	CortePenalizacion decimal (10,2) null	" +
                        "	) ENGINE=InnoDB;	";

                        string create_pensiones = "	CREATE TABLE Pensiones(	" +
                        "	ID_Tipo_Pension int AUTO_INCREMENT PRIMARY KEY not NULL,	" +
                        "	Pensn_Tipo varchar(100) NULL,	" +
                        "	Pens_Bonificacion decimal (10,2) NULL,	" +
                        "	Pens_Costo_Regular decimal (10,2) NULL,	" +
                        "	Pens_Tolerancia_dias int NULL,	" +
                        "	Pens_Recargos decimal (10,2) NULL,	" +
                        "	Pens_Cobro_1 int NULL,	" +
                        "	Pens_Cobro_2 int NULL,	" +
                        "	Pens_DiasInactivo int NULL	" +
                        "	) ENGINE=InnoDB;	";

                        string create_vehiculo = "	create table Vehiculo(	" +
                        "	Id_Vehiculo int AUTO_INCREMENT primary key not null,	" +
                        "	Marca varchar(30) not null,	" +
                        "	Placa varchar(10) not null,	" +
                        "	Color varchar(30) not null,	" +
                        "	Descripcion varchar(200)	" +
                        "	) ENGINE=InnoDB;	";

                        string create_cliente = "	create table Cliente(	" +
                        "	Id_cliente int AUTO_INCREMENT primary key not null, 	" +
                        "	Nombre varchar(100) not null, 	" +
                        "	Apellido_paterno varchar(100) not null,	" +
                        "	Apellido_materno varchar(100) not null,	" +
                        "	Tel1 varchar(10) not null,	" +
                        "	Tel2 varchar(10) not null,	" +
                        "	Direccion varchar(200) not null,	" +
                        "	Correo varchar(100) not null,	" +
                        "	RFC varchar(10) not null,	" +
                        "	Razon_Social varchar(100)null,	" +
                        "	Id_Vehiculo int not null,	" +
                        "	foreign key(Id_Vehiculo) references Vehiculo(Id_Vehiculo)	" +
                        "	) ENGINE=InnoDB;	";

                        string create_bancopension = "	CREATE TABLE Banco_Pension(	" +
                        "	ID int AUTO_INCREMENT primary key not NULL,	" +
                        "	Folio varchar (20)null,	" +
                        "	FechaInscripcion date NULL,	" +
                        "	FechaUltimoPago date NULL,	" +
                        "	CobradoPor varchar(50) NULL,	" +
                        "	Inicio_Pension date NULL,	" +
                        "	Fin_Pension date NULL,	" +
                        "	ID_Tipo_Pension int NOT NULL, 	" +
                        "	    foreign key (ID_Tipo_Pension) references Pensiones(ID_Tipo_Pension),    	" +
                        "	StatusPago bit NULL,	" +
                        "	No_PagosAnticipados int NULL,	" +
                        "	Costo decimal (10,2) NULL,	" +
                        "	Pagos decimal (10,2) NULL,	" +
                        "	Id_cliente int references Cliente,	" +
                        "	Pension_Corte bit NULL,	" +
                        "	Pension_Activa bit NULL,	" +
                        "	Id_corte int references Cortes	" +
                        "	)ENGINE=InnoDB;	";

                        string create_areas = "	CREATE TABLE Areas_Personal(	" +
                        "	ID_AreaPersonal int AUTO_INCREMENT PRIMARY KEY NOT NULL,	" +
                        "	AreaPersonal varchar(100) NULL	" +
                        "	)ENGINE=InnoDB; 	" +
                        "	CREATE TABLE ProveedoresCorreo(	" +
                        "	ID_ProveedoresCorreo int AUTO_INCREMENT primary key not NULL,	" +
                        "	Dominio varchar(50) NULL,	" +
                        "	Servidor_SMTP varchar(200) NULL,	" +
                        "	Puerto int NULL	" +
                        "	)ENGINE=InnoDB;  	";

                        string create_email = "	CREATE TABLE Email(	" +
                        "	ID_Email int  AUTO_INCREMENT primary key not NULL,	" +
                        "	    Direccion_Email varchar(150) NULL,	" +
                        "	    Password varchar(150) NULL,	" +
                        "	    Asunto varchar(150) NULL,	" +
                        "	Cuerpo varchar(2000) NULL,	" +
                        "	Url_Encabezado longblob null, 	" +
                        "	Nombre varchar(800) NULL,	" +
                        "	Direccion varchar(500) NULL,	" +
                        "	Correo varchar(800) NULL,	" +
                        "	Telefono varchar(10) NULL,	" +
                        "	WebSite varchar(800) NULL,	" +
                        "	    ID_ProveedoresCorreo int NULL,	" +
                        "	    foreign key(ID_ProveedoresCorreo) references ProveedoresCorreo(ID_ProveedoresCorreo)	" +
                        "	) ENGINE=InnoDB;	";

                        string create_config = "	CREATE TABLE ConfigGen1 (	" +
                        "	    ID_config INT AUTO_INCREMENT PRIMARY KEY NOT NULL,	" +
                        "	    PeriodoCobro BIT NOT NULL,	" +
                        "	    Tolerancia_ingreso INT NOT NULL,	" +
                        "	    Tolerancia_proxtarifa INT NOT NULL,	" +
                        "	    Ticket_cancel BIT NOT NULL,	" +
                        "	    TCancel INT NOT NULL,	" +
                        "	    Ticket_cortesia BIT NOT NULL,	" +
                        "	    Tcortesia INT NOT NULL,	" +
                        "	    Ticker_perdido BIT NOT NULL,	" +
                        "	    Tperdido INT NOT NULL,	" +
                        "	    Multa INT NOT NULL,	" +
                        "	    Capacidad INT NOT NULL,	" +
                        "	    Buscar_vehiculo BIT NOT NULL,	" +
                        "	    Modo_cobro BIT NOT NULL,	" +
                        "	    Limpiar INT NOT NULL,	" +
                        "	    DireccionFactura varchar(100) null	" +
                        "	)  ENGINE=INNODB; 	";

                        string create_usuario = "	CREATE TABLE Usuario(	" +
                        "	ID_Usuario int AUTO_INCREMENT primary key not NULL,	" +
                        "	Privilegios varchar(50) NULL,	" +
                        "	Usuario varchar(500) NULL,	" +
                        "	Password varchar(500) NULL,	" +
                        "	Nombre varchar(100) NULL,	" +
                        "	Telefono varchar(50) NULL,	" +
                        "	ID_AreaPersonal int NOT NULL,	" +
                        "	    foreign key(ID_AreaPersonal) references Areas_Personal(ID_AreaPersonal),	" +
                        "	Puesto varchar(100) NULL,	" +
                        "	mail_Correo varchar(100) NULL,	" +
                        "	mail_Password varchar(100) NULL,	" +
                        "	ID_ProveedoresCorreo int NULL,	" +
                        "	    foreign key(ID_ProveedoresCorreo) references ProveedoresCorreo(ID_ProveedoresCorreo),	" +
                        "	CorreoConfirmado bit NULL	" +
                        "	)ENGINE=InnoDB;	";

                        string create_tpensionesu = "	CREATE TABLE Tipos_PensionesU(	" +
                        "	ID_Tipo_PensionU int AUTO_INCREMENT primary key not NULL,	" +
                        "	Tipo_PensionU varchar(100) NULL,	" +
                        "	HoraInicio time NULL,	" +
                        "	HoraFin time NULL,	" +
                        "	Tolerancia_IN int NULL,	" +
                        "	Tolerancia_OUT int NULL	" +
                        "	)ENGINE=InnoDB;	";

                        string create_pensu = "	CREATE TABLE Pensiones_Unicas(	" +
                        "	ID_PensionU int AUTO_INCREMENT primary key not NULL,	" +
                        "	Nombre_PensionU varchar(100) NULL,	" +
                        "	ID_Tipo_PensionU int NOT NULL,	" +
                        "	    foreign key(ID_Tipo_PensionU) references Tipos_PensionesU(ID_Tipo_PensionU),	" +
                        "	PensionU_Activa bit NULL,	" +
                        "	Precio_PensionU decimal (10,2) NULL	" +
                        "	)ENGINE=InnoDB;	";

                        string create_penaliza = "	CREATE TABLE Penalizaciones(	" +
                        "	ID_Penalizacion int AUTO_INCREMENT primary key  not NULL,	" +
                        "	Penalizacion varchar(100) NULL,	" +
                        "	Penalizacion_Activa bit NULL,	" +
                        "	Precio_Penalizacion decimal (10,2) NULL,	" +
                        "	Penalizacion_Tolerancia int NULL	" +
                        "	)ENGINE=InnoDB;	";

                        string create_servia = "	CREATE TABLE ServiciosAdicionales(	" +
                        "	ID_ServicioAd int AUTO_INCREMENT primary key not NULL,	" +
                        "	ServicioAdicional varchar(100) NULL,	" +
                        "	ServicioAd_Activo bit NULL,	" +
                        "	Precio_ServiciosAd decimal (10,2) NULL,	" +
                        "	Tiempo_Gracia int NULL	" +
                        "	)ENGINE=InnoDB; 	";

                        string create_cobro = "	CREATE TABLE Cobro(	" +
                        "	ID_Tabulador int AUTO_INCREMENT primary key not NULL,	" +
                        "	Nombre varchar(100) NULL,	" +
                        "	Tarifa_Habilitada bit NULL,	" +
                        "	FormaCobro bit NULL,	" +
                        "	TipoCobro bit null,	" +
                        "	CobroLineal_ImporteDefault decimal (10,2) NULL,	" +
                        "	CobroLineal_minDefault int NULL,	" +
                        "	CobroLineal_Importe decimal (10,2) NULL,	" +
                        "	CobroLineal_minFrecuencia int NULL,	" +
                        "	min_30 decimal (10,2) NULL,	" +
                        "	min_60 decimal (10,2) NULL,	" +
                        "	min_90 decimal (10,2) NULL,	" +
                        "	min_120 decimal (10,2) NULL,	" +
                        "	min_150 decimal (10,2) NULL,	" +
                        "	min_180 decimal (10,2) NULL,	" +
                        "	min_210 decimal (10,2) NULL,	" +
                        "	min_240 decimal (10,2) NULL,	" +
                        "	min_270 decimal (10,2) NULL,	" +
                        "	min_300 decimal (10,2) NULL,	" +
                        "	min_330 decimal (10,2) NULL,	" +
                        "	min_360 decimal (10,2) NULL,	" +
                        "	min_390 decimal (10,2) NULL,	" +
                        "	min_420 decimal (10,2) NULL,	" +
                        "	min_450 decimal (10,2) NULL,	" +
                        "	min_480 decimal (10,2) NULL,	" +
                        "	min_510 decimal (10,2) NULL,	" +
                        "	min_540 decimal (10,2) NULL,	" +
                        "	min_570 decimal (10,2) NULL,	" +
                        "	min_600 decimal (10,2) NULL,	" +
                        "	min_630 decimal (10,2) NULL,	" +
                        "	min_660 decimal (10,2) NULL,	" +
                        "	min_690 decimal (10,2) NULL,	" +
                        "	min_720 decimal (10,2) NULL,	" +
                        "	min_750 decimal (10,2) NULL,	" +
                        "	min_780 decimal (10,2) NULL,	" +
                        "	min_810 decimal (10,2) NULL,	" +
                        "	min_840 decimal (10,2) NULL,	" +
                        "	min_870 decimal (10,2) NULL,	" +
                        "	min_900 decimal (10,2) NULL,	" +
                        "	min_930 decimal (10,2) NULL,	" +
                        "	min_960 decimal (10,2) NULL,	" +
                        "	min_990 decimal (10,2) NULL,	" +
                        "	min_1020 decimal (10,2) NULL,	" +
                        "	min_1050 decimal (10,2) NULL,	" +
                        "	min_1080 decimal (10,2) NULL,	" +
                        "	min_1110 decimal (10,2) NULL,	" +
                        "	min_1140 decimal (10,2) NULL,	" +
                        "	min_1170 decimal (10,2) NULL,	" +
                        "	min_1200 decimal (10,2) NULL,	" +
                        "	min_1230 decimal (10,2) NULL,	" +
                        "	min_1260 decimal (10,2) NULL,	" +
                        "	min_1290 decimal (10,2) NULL,	" +
                        "	min_1320 decimal (10,2) NULL,	" +
                        "	min_1350 decimal (10,2) NULL,	" +
                        "	min_1380 decimal (10,2) NULL,	" +
                        "	min_1410 decimal (10,2) NULL,	" +
                        "	min_1440 decimal (10,2) NULL	" +
                        "	)ENGINE=InnoDB; 	";

                        string create_bparking = "	CREATE TABLE Banco_Parking(	" +
                        "	ID int AUTO_INCREMENT PRIMARY KEY NOT NULL,	" +
                        "	Folio varchar(20),	" +
                        "	    FechaEntrada date NULL,	" +
                        "	HoraEntrada time NULL,	" +
                        "	ExpedidoPor varchar(50) NULL,	" +
                        "	Placa varchar(50) NULL,	" +
                        "	Marca varchar(50) NULL,	" +
                        "	Color varchar(50) NULL,	" +
                        "	ID_PensionU int NOT NULL, foreign key(ID_PensionU) references Pensiones_Unicas(ID_PensionU),	" +
                        "	ID_Cobro int NOT NULL, foreign key(ID_Cobro) references Cobro(ID_Tabulador),	" +
                        "	ID_ServicioAd int NOT NULL, foreign key(ID_ServicioAd) references ServiciosAdicionales(ID_ServicioAd) ,	" +
                        "	ID_Penalizacion int NOT NULL, foreign key(ID_Penalizacion) references Penalizaciones(ID_Penalizacion),	" +
                        "	FechaSalida date NULL,	" +
                        "	HoraSalida time NULL, 	" +
                        "	CobradoPor varchar(50) NULL,	" +
                        "	T_Parqueo time NULL,	" +
                        "	Estancia varchar(100) NULL,	" +
                        "	Importe decimal (10,2) NULL,	" +
                        "	Pago decimal (10,2) NULL,	" +
                        "	ModoCobro varchar(50) NULL,	" +
                        "	ModoTicket varchar(50) NULL,	" +
                        "	Justificacion varchar(500) NULL,	" +
                        "	Corte int NULL,	" +
                        "	Semana int NULL,	" +
                        "	Mes int NULL,	" +
                        "	Id_corte int,foreign key(Id_corte) references Cortes(Id_Corte)	" +
                        "	)ENGINE=InnoDB; 	";

                        string create_bitacora = "	CREATE TABLE Bitacora_Pensiones (	" +
                        "	    Id_bitacora INT AUTO_INCREMENT PRIMARY KEY NOT NULL,	" +
                        "	    Fecha_Entrada DATE NOT NULL,	" +
                        "	    Fecha_salida date null,	" +
                        "	    Hora_Entrada TIME NOT NULL,	" +
                        "	    Hora_Salida TIME NULL,	" +
                        "	    Tiempo_Exedido TIME NULL,	" +
                        "	    Penalizacion BOOL NOT NULL,	" +
                        "	    Monto_Penalizacion DECIMAL(10 , 2 ) NULL,	" +
                        "	    Id_Pension INT,	" +
                        "	    FOREIGN KEY (Id_Pension)	" +
                        "	        REFERENCES Banco_Pension (ID)	" +
                        "	)  ENGINE=INNODB;";


                        #endregion

                        #region script insert
                        string inserts = "	insert into Areas_Personal (AreaPersonal) values('Administracion'),	" +
                        "	('Almacen'),	" +
                        "	('Facturacion'),	" +
                        "	('Otros'),	" +
                        "	('Ventas');	" +
                        "	insert into ProveedoresCorreo(Dominio,Servidor_SMTP,Puerto) values('@gmail.com', 'smtp.gmail.com', 587),	" +
                        "	('@yahoo.com', 'smtp.mail.yahoo.com', 587),	" +
                        "	('@hotmail.com', 'smtp.live.com', 587),	" +
                        "	('@outlook.com', 'smtp.live.com', 587),	" +
                        "	('@1and1.com', 'smtp.1and1.com', 587);	" +
                        "	insert into Tipos_PensionesU(Tipo_PensionU,HoraInicio,HoraFin,Tolerancia_IN,Tolerancia_OUT) values ('Diurna', '08:00', '20:30', 30,30);	" +
                        "	insert into Tipos_PensionesU(Tipo_PensionU,HoraInicio,HoraFin,Tolerancia_IN,Tolerancia_OUT)  values ('Nocturna', '20:30', '08:00', 30,30);	" +
                        "	Insert into Tipos_pensionesU(Tipo_PensionU,HoraInicio,HoraFin,Tolerancia_IN,Tolerancia_OUT) values('Ninguna', '00:00', '00:00',0,0);	" +
                        "	insert into Pensiones_Unicas(Nombre_PensionU,ID_Tipo_PensionU,PensionU_Activa,Precio_PensionU)values ('DIU-TIPO_01',1,0, 120.00);	" +
                        "	insert into Pensiones_Unicas(Nombre_PensionU,ID_Tipo_PensionU,PensionU_Activa,Precio_PensionU) values ('NOC-TIPO_01',2,0, 100.00);	" +
                        "	insert into Pensiones_Unicas (Nombre_PensionU,ID_Tipo_PensionU,PensionU_Activa,Precio_PensionU)values ('DIU-TIPO_02',1,0, 110.00);	" +
                        "	insert into Pensiones_Unicas(Nombre_PensionU,ID_Tipo_PensionU,PensionU_Activa,Precio_PensionU) values ('NOC-TIPO_02',2,0, 90.00);	" +
                        "	insert into Pensiones_Unicas(Nombre_PensionU,ID_Tipo_PensionU,PensionU_Activa,Precio_PensionU) values ('DIU-TIPO_03',1,0, 100.00);	" +
                        "	insert into Pensiones_Unicas(Nombre_PensionU,ID_Tipo_PensionU,PensionU_Activa,Precio_PensionU) values ('NOC-TIPO_03',2,0, 80.00);	" +
                        "	insert into Pensiones_Unicas(Nombre_PensionU,ID_Tipo_PensionU,PensionU_Activa,Precio_PensionU) values ('DIU-TIPO_04',1,0, 90.00);	" +
                        "	insert into Pensiones_Unicas(Nombre_PensionU,ID_Tipo_PensionU,PensionU_Activa,Precio_PensionU) values ('NOC-TIPO_04',2,0, 70.00);	" +
                        "	insert into Pensiones_Unicas(Nombre_PensionU,ID_Tipo_PensionU,PensionU_Activa,Precio_PensionU) values('Ninguna', 3 , 1 , 0.00);	" +
                        "	insert into Penalizaciones(Penalizacion,Penalizacion_Activa,Precio_Penalizacion,Penalizacion_Tolerancia) values ('Ninguna', 1 , 0.00, 9999);	" +
                        "	insert into ServiciosAdicionales(ServicioAdicional,ServicioAd_Activo,Precio_ServiciosAd,Tiempo_Gracia) values('Ninguno', 0, 00.00, 0);	" +
                        "	insert into Penalizaciones(Penalizacion,Penalizacion_Activa,Precio_Penalizacion,Penalizacion_Tolerancia) values ('CORTESÍA', 1 , 0.00, 9999);	" +
                        "	insert into Penalizaciones(Penalizacion,Penalizacion_Activa,Precio_Penalizacion,Penalizacion_Tolerancia) values ('TICKET PERDIDO', 1 , 50.00, 9999);	" +
                        "	insert into Penalizaciones(Penalizacion,Penalizacion_Activa,Precio_Penalizacion,Penalizacion_Tolerancia) values ('TICKET CANCELADO', 1 , 0.00, 15);	" +
                        "	insert into ServiciosAdicionales(ServicioAdicional,ServicioAd_Activo,Precio_ServiciosAd,Tiempo_Gracia)values('Lavado Auto', 0, 50.00, 0);	" +
                        "	insert into Email(Direccion_Email,Password,Asunto,Cuerpo,Url_Encabezado,Nombre,Direccion,Correo,Telefono,WebSite,ID_ProveedoresCorreo) values	" +
                        "	 ('Correo@Correo.com','Password','Asunto', 'Cuerpo','','Nombre','Direccion','Correo','Telefono','Website.com',1);	" +
                        "	insert into Cobro(Nombre,Tarifa_Habilitada,FormaCobro,TipoCobro,CobroLineal_ImporteDefault,CobroLineal_minDefault,CobroLineal_Importe,CobroLineal_minFrecuencia,min_30,min_60,min_90,min_120,min_150,min_180,min_210,min_240,min_270,min_300,min_330,min_360,min_390,min_420,min_450,min_480,min_510,min_540,min_570,min_600,min_630,min_660,min_690,min_720,min_750,min_780,min_810,min_840,min_870,min_900,min_930,min_960,min_990,min_1020,min_1050,min_1080,min_1110,min_1140,min_1170,min_1200,min_1230,min_1260,min_1290,min_1320,min_1350,min_1380,min_1410,min_1440) values ('TIENDA' , 0 , 1,0, 0.00 , 0 , 5.00 , 60 , 5.00 , 5.00 , 10.00 , 10.00 , 15.00 , 15.00 , 20.00 , 20.00 , 25.00 , 25.00 , 30.00 , 30.00 , 35.00 , 35.00 , 40.00 , 40.00 , 45.00 , 45.00 , 50.00 , 50.00 , 55.00 , 55.00 , 60.00 , 60.00 , 65.00 , 65.00 , 70.00 , 70.00 , 75.00 , 75.00 , 80.00 , 80.00 , 85.00 , 85.00 , 90.00 , 90.00 , 95.00 , 95.00 , 100.00 , 100.00 , 105.00 , 105.00 , 110.00 , 110.00 , 115.00 , 115.00 , 120.00 , 120.00);	" +
                        "	insert into Cobro(Nombre,Tarifa_Habilitada,FormaCobro,TipoCobro,CobroLineal_ImporteDefault,CobroLineal_minDefault,CobroLineal_Importe,CobroLineal_minFrecuencia,min_30,min_60,min_90,min_120,min_150,min_180,min_210,min_240,min_270,min_300,min_330,min_360,min_390,min_420,min_450,min_480,min_510,min_540,min_570,min_600,min_630,min_660,min_690,min_720,min_750,min_780,min_810,min_840,min_870,min_900,min_930,min_960,min_990,min_1020,min_1050,min_1080,min_1110,min_1140,min_1170,min_1200,min_1230,min_1260,min_1290,min_1320,min_1350,min_1380,min_1410,min_1440) values ('HOTEL' , 0 , 1,0, 0.00 , 0 , 5.00 , 60 , 5.00 , 5.00 , 10.00 , 10.00 , 15.00 , 15.00 , 20.00 , 20.00 , 25.00 , 25.00 , 30.00 , 30.00 , 35.00 , 35.00 , 40.00 , 40.00 , 45.00 , 45.00 , 50.00 , 50.00 , 55.00 , 55.00 , 60.00 , 60.00 , 65.00 , 65.00 , 70.00 , 70.00 , 75.00 , 75.00 , 80.00 , 80.00 , 85.00 , 85.00 , 90.00 , 90.00 , 95.00 , 95.00 , 100.00 , 100.00 , 105.00 , 105.00 , 110.00 , 110.00 , 115.00 , 115.00 , 120.00 , 120.00);	" +
                        "	insert into Cobro(Nombre,Tarifa_Habilitada,FormaCobro,TipoCobro,CobroLineal_ImporteDefault,CobroLineal_minDefault,CobroLineal_Importe,CobroLineal_minFrecuencia,min_30,min_60,min_90,min_120,min_150,min_180,min_210,min_240,min_270,min_300,min_330,min_360,min_390,min_420,min_450,min_480,min_510,min_540,min_570,min_600,min_630,min_660,min_690,min_720,min_750,min_780,min_810,min_840,min_870,min_900,min_930,min_960,min_990,min_1020,min_1050,min_1080,min_1110,min_1140,min_1170,min_1200,min_1230,min_1260,min_1290,min_1320,min_1350,min_1380,min_1410,min_1440) values ('C. COMERCIAL' , 1,0, 0 , 0.00 , 0 , 5.00 , 60 , 5.00 , 5.00 , 10.00 , 10.00 , 15.00 , 15.00 , 20.00 , 20.00 , 25.00 , 25.00 , 30.00 , 30.00 , 35.00 , 35.00 , 40.00 , 40.00 , 45.00 , 45.00 , 50.00 , 50.00 , 55.00 , 55.00 , 60.00 , 60.00 , 65.00 , 65.00 , 70.00 , 70.00 , 75.00 , 75.00 , 80.00 , 80.00 , 85.00 , 85.00 , 90.00 , 90.00 , 95.00 , 95.00 , 100.00 , 100.00 , 105.00 , 105.00 , 110.00 , 110.00 , 115.00 , 115.00 , 120.00 , 120.00);	" +
                        "	insert into Cobro(Nombre,Tarifa_Habilitada,FormaCobro,TipoCobro,CobroLineal_ImporteDefault,CobroLineal_minDefault,CobroLineal_Importe,CobroLineal_minFrecuencia,min_30,min_60,min_90,min_120,min_150,min_180,min_210,min_240,min_270,min_300,min_330,min_360,min_390,min_420,min_450,min_480,min_510,min_540,min_570,min_600,min_630,min_660,min_690,min_720,min_750,min_780,min_810,min_840,min_870,min_900,min_930,min_960,min_990,min_1020,min_1050,min_1080,min_1110,min_1140,min_1170,min_1200,min_1230,min_1260,min_1290,min_1320,min_1350,min_1380,min_1410,min_1440) values('AUTO' , 1 ,1, 1 , 0.00 , 0 , 7.00 , 60 , 7.00 , 7.00 , 14.00 , 14.00 , 21.00 , 21.00 , 28.00 , 28.00 , 35.00 , 35.00 , 42.00 , 42.00 , 49.00 , 49.00 , 56.00 , 56.00 , 63.00 , 63.00 , 70.00 , 70.00 , 77.00 , 77.00 , 84.00 , 84.00 , 91.00 , 91.00 , 98.00 , 98.00 , 105.00 , 105.00 , 112.00 , 112.00 , 119.00 , 119.00 , 126.00 , 126.00 , 133.00 , 133.00 , 140.00 , 140.00 , 147.00 , 147.00 , 154.00 , 154.00 , 161.00 , 161.00 , 168.00 , 168.00);	" +
                        "	insert into Cobro(Nombre,Tarifa_Habilitada,FormaCobro,TipoCobro,CobroLineal_ImporteDefault,CobroLineal_minDefault,CobroLineal_Importe,CobroLineal_minFrecuencia,min_30,min_60,min_90,min_120,min_150,min_180,min_210,min_240,min_270,min_300,min_330,min_360,min_390,min_420,min_450,min_480,min_510,min_540,min_570,min_600,min_630,min_660,min_690,min_720,min_750,min_780,min_810,min_840,min_870,min_900,min_930,min_960,min_990,min_1020,min_1050,min_1080,min_1110,min_1140,min_1170,min_1200,min_1230,min_1260,min_1290,min_1320,min_1350,min_1380,min_1410,min_1440)values('CAMIONETA' , 0,1 , 1 , 0.00 , 0 , 8.00 , 60 , 8.00 , 8.00 , 16.00 , 16.00 , 24.00 , 24.00 , 32.00 , 32.00 , 40.00 , 40.00 , 48.00 , 48.00 , 56.00 , 56.00 , 64.00 , 64.00 , 72.00 , 72.00 , 80.00 , 80.00 , 88.00 , 88.00 , 96.00 , 96.00 , 104.00 , 104.00 , 112.00 , 112.00 , 120.00 , 120.00 , 128.00 , 128.00 , 136.00 , 136.00 , 144.00 , 144.00 , 152.00 , 152.00 , 160.00 , 160.00 , 168.00 , 168.00 , 176.00 , 176.00 , 184.00 , 184.00 , 192.00 , 192.00);	" +
                        "	insert into Cobro(Nombre,Tarifa_Habilitada,FormaCobro,TipoCobro,CobroLineal_ImporteDefault,CobroLineal_minDefault,CobroLineal_Importe,CobroLineal_minFrecuencia,min_30,min_60,min_90,min_120,min_150,min_180,min_210,min_240,min_270,min_300,min_330,min_360,min_390,min_420,min_450,min_480,min_510,min_540,min_570,min_600,min_630,min_660,min_690,min_720,min_750,min_780,min_810,min_840,min_870,min_900,min_930,min_960,min_990,min_1020,min_1050,min_1080,min_1110,min_1140,min_1170,min_1200,min_1230,min_1260,min_1290,min_1320,min_1350,min_1380,min_1410,min_1440) values('MOTO' , 0 , 1,1 , 0.00 , 0 , 6.00 , 60 , 6.00 , 6.00 , 12.00 , 12.00 , 18.00 , 18.00 , 24.00 , 24.00 , 30.00 , 30.00 , 36.00 , 36.00 , 42.00 , 42.00 , 48.00 , 48.00 , 54.00 , 54.00 , 60.00 , 60.00 , 66.00 , 66.00 , 72.00 , 72.00 , 78.00 , 78.00 , 84.00 , 84.00 , 90.00 , 90.00 , 96.00 , 96.00 , 102.00 , 102.00 , 108.00 , 108.00 , 114.00 , 114.00 , 120.00 , 120.00 , 126.00 , 126.00 , 132.00 , 132.00 , 138.00 , 138.00 , 144.00 , 144.00);	" +
                        "	insert into Cobro(Nombre,Tarifa_Habilitada,FormaCobro,TipoCobro,CobroLineal_ImporteDefault,CobroLineal_minDefault,CobroLineal_Importe,CobroLineal_minFrecuencia,min_30,min_60,min_90,min_120,min_150,min_180,min_210,min_240,min_270,min_300,min_330,min_360,min_390,min_420,min_450,min_480,min_510,min_540,min_570,min_600,min_630,min_660,min_690,min_720,min_750,min_780,min_810,min_840,min_870,min_900,min_930,min_960,min_990,min_1020,min_1050,min_1080,min_1110,min_1140,min_1170,min_1200,min_1230,min_1260,min_1290,min_1320,min_1350,min_1380,min_1410,min_1440) values('TAXI' , 0 , 1 ,1, 0.00 , 0 , 5.00 , 60 , 5.00 , 5.00 , 10.00 , 10.00 , 15.00 , 15.00 , 20.00 , 20.00 , 25.00 , 25.00 , 30.00 , 30.00 , 35.00 , 35.00 , 40.00 , 40.00 , 45.00 , 45.00 , 50.00 , 50.00 , 55.00 , 55.00 , 60.00 , 60.00 , 65.00 , 65.00 , 70.00 , 70.00 , 75.00 , 75.00 , 80.00 , 80.00 , 85.00 , 85.00 , 90.00 , 90.00 , 95.00 , 95.00 , 100.00 , 100.00 , 105.00 , 105.00 , 110.00 , 110.00 , 115.00 , 115.00 , 120.00 , 120.00);	" +
                        "	insert into pensiones(Pensn_Tipo,Pens_Bonificacion,Pens_Costo_Regular,Pens_Tolerancia_dias,Pens_Recargos,Pens_Cobro_1,Pens_Cobro_2,Pens_DiasInactivo) values ('Semanal', 0.00 ,500.00 ,0 ,0.00 ,1 ,16, 5);	" +
                        "	insert into pensiones(Pensn_Tipo,Pens_Bonificacion,Pens_Costo_Regular,Pens_Tolerancia_dias,Pens_Recargos,Pens_Cobro_1,Pens_Cobro_2,Pens_DiasInactivo) values ('Quincenal', 0.00 ,900.00, 0 ,0.00, 1 ,16, 5);	" +
                        "	insert into pensiones(Pensn_Tipo,Pens_Bonificacion,Pens_Costo_Regular,Pens_Tolerancia_dias,Pens_Recargos,Pens_Cobro_1,Pens_Cobro_2,Pens_DiasInactivo) values ('Mensual', 0.00 ,1500.00 ,0  ,0.00 ,1, 16, 5);	" +
                        "	insert into pensiones(Pensn_Tipo,Pens_Bonificacion,Pens_Costo_Regular,Pens_Tolerancia_dias,Pens_Recargos,Pens_Cobro_1,Pens_Cobro_2,Pens_DiasInactivo) values ('Anual', 0.00 ,5000.00 ,0 ,0.00 ,1 ,16, 5);	" +
                        "	insert into Usuario(Privilegios,Usuario,Password,Nombre,Telefono,ID_AreaPersonal,Puesto,mail_Correo,mail_Password,ID_ProveedoresCorreo,CorreoConfirmado) values ('Administrador', 'a' , 'a', 'a','0000000000',5, 'EMPLEADO2', 'CORREO@GMAIL.COM', 'EJEJE', 1,'');	" +
                        "	insert into ConfigGen1(PeriodoCobro,Tolerancia_ingreso,Tolerancia_proxtarifa,Ticket_cancel,TCancel,Ticket_cortesia,Tcortesia,Ticker_perdido,Tperdido,Multa,Capacidad,Buscar_vehiculo,Modo_cobro,Limpiar,DireccionFactura) values (0,5,5,1,15,1,9999,1,9999,50,50,0,1,4,'');	" +
                        "	insert into Config_Tickets(Incluir_Logo,Tamaño_papel,Estacionamiento,Direccion,Razon_Social,Telefono,RFC,DatosEntrada,DatosSalida,DatosPerdido,DatosCortesia,DatosConvenio,DatosCancelado,DatosPension,Imagen,Cantidad_Copias_Entrada,Cantidad_Copias_Salida,Desarrollador) values 	" +
                        "	(0, 0, 'Estacionamiento','Direccion','Razon Social','Telefono','RFC',	" +
                        "	'El cliente se notifica y acepta que el estacionamiento del vehiculo no implica contrato de deposito y que el propietario se hace responsable por objetos de valor dejados dentro del vehiculo. Tampoco nos hacemos responsables por: Robo de objetos, valores, documentos, autopartes o accesorios no declarados y/o depositados en la caseta de cobro, ni daños relacionados con colisiones o accidentes, terremotos, incendios, inundaciones, alborotos populares, vandalismo, daños mecanicos o electricos, ponchadura de llantas o ruptura de cristales. En caso de daños parciales por causas imputables a la empresa, ésta respondera de acuerdo a las politicas de la compañia de seguros. El usuario es responsable y pagara por cualquier daño fisico al estacionamiento, equipos, vehiculos o personas. Una ves que el vehiculo sea retirado del estacionamiento no se acepta reclamacion alguna.El recibir este boleto implica la aceptacion del contrato y de no estar de acuerdo con las clausulas anteriores favor de retirar su vehiculo. Este ticket es el unico documento valido de salida. Cuide su boleto, perderlo implica acreditar la propiedad de su vehiculo',	" +
                        "	'',	" +
                        "	'EN LA CIUDAD DE PUEBLA,PUE. SIENDO LAS __ DEL DIA_____LA(EL) C.______ IDENTIFICANDOSE CON ____ EXPEDIDA POR____ QUIEN DICE SER EL PROPIETARIO DEL VEHICULO MODELO __ COLOR___ PLACAS____ MOSTRANDO LA TARJETA DE CIRCULACION Y COPIA DE SU IFE, MANIFIESTA EL EXTRAVIO DEL BOLETO DE ACCESO AL ESTACIONAMIENTO, POR LO QUE SE PROCEDE A LEVANTAR LA PRESENTE CONSTANCIA Y A EFECTUAR EL COBRO RESPECTIVO DE $50.00 (CINCUENTA PESOS 00/100 M.N.) MAS EL IMPORTE DE HORAS EN EL ESTACIONAMIENTO PARA SU REPOSICION.',	" +
                        "	'',	" +
                        "	'',	" +
                        "	'',	" +
                        "	'','',1,2,'Desarrollado por: Ideastro');	" +
                        "	insert into cortes(Usuario,FechaCorte,Importe_Parqueo,Importe_Cortes,Importe_Reportado,StatusCorte,CorteNormal,CorteTarifa2,CortePension) values('ADMIN', null, 0,0,0,0,0,0,0);	";

                        #endregion
                        try
                        {
                            MySqlConnection conn = new MySqlConnection(connStrNueva);
                            myConnection.Open();
                            MySqlCommand cmdd = conn.CreateCommand();
                            MySqlCommand cmd = myConnection.CreateCommand();
                            cmd.CommandText = create_base;
                            cmd.ExecuteNonQuery();
                            myConnection.Close();
                            conn.Open();
                            cmdd.CommandText = create_configtickets;
                            cmdd.ExecuteNonQuery();
                            cmdd.CommandText = create_cortes;
                            cmdd.ExecuteNonQuery();
                            cmdd.CommandText = create_pensiones;
                            cmdd.ExecuteNonQuery();
                            cmdd.CommandText = create_vehiculo;
                            cmdd.ExecuteNonQuery();
                            cmdd.CommandText = create_cliente;
                            cmdd.ExecuteNonQuery();
                            cmdd.CommandText = create_bancopension;
                            cmdd.ExecuteNonQuery();
                            cmdd.CommandText = create_areas;
                            cmdd.ExecuteNonQuery();
                            cmdd.CommandText = create_email;
                            cmdd.ExecuteNonQuery();
                            cmdd.CommandText = create_config;
                            cmdd.ExecuteNonQuery();
                            cmdd.CommandText = create_usuario;
                            cmdd.ExecuteNonQuery();
                            cmdd.CommandText = create_tpensionesu;
                            cmdd.ExecuteNonQuery();
                            cmdd.CommandText = create_pensu;
                            cmdd.ExecuteNonQuery();
                            cmdd.CommandText = create_penaliza;
                            cmdd.ExecuteNonQuery();
                            cmdd.CommandText = create_servia;
                            cmdd.ExecuteNonQuery();
                            cmdd.CommandText = create_cobro;
                            cmdd.ExecuteNonQuery();
                            cmdd.CommandText = create_bparking;
                            cmdd.ExecuteNonQuery();
                            cmdd.CommandText = create_bitacora;
                            cmdd.ExecuteNonQuery();
                            cmdd.CommandText = inserts;
                            cmdd.ExecuteNonQuery();
                            EntityConnectionStringBuilder connectionStringBuilder = new EntityConnectionStringBuilder();
                            connectionStringBuilder.Provider = "MySql.Data.MySqlClient";
                            connectionStringBuilder.ProviderConnectionString = "server=" + servidor + ";user id=" + user + ";password=" + pass + ";persistsecurityinfo=True;database=parking_db";
                            connectionStringBuilder.Metadata = "res://*/EntityModel.csdl|res://*/EntityModel.ssdl|res://*/EntityModel.msl";
                            Conexion = connectionStringBuilder.ToString();

                            ModeloDatos oSettings = JsonConvert.DeserializeObject<ModeloDatos>(File.ReadAllText(rutaJson));
                            oSettings.CadenaConexion = Conexion;
                            oSettings.Serial = Serial;
                            oSettings.Tipo = "Enterprise";
                            var hoy = DateTime.Today.ToString();
                            oSettings.FechaInicio = hoy;
                            var termino = (DateTime.Today.AddDays(7)).ToString();
                            oSettings.FechaTermino = termino;
                            var json = JsonConvert.SerializeObject(oSettings, Formatting.None);
                            File.WriteAllText(rutaJson, json);
                            var result = MessageBox.Show("Base de datos creada con exito", "Base de datos creada con exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            if (result == DialogResult.OK)
                            {
                                Process.Start(Application.ExecutablePath);
                                Application.Exit();
                            }

                        }
                        catch (Exception ex)
                        {
                            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                            string filePath = path + @"\Error.txt";
                            using (StreamWriter writer = new StreamWriter(filePath, true))
                            {
                                writer.WriteLine("-----------------------------------------------------------------------------");
                                writer.WriteLine("Date : " + DateTime.Now.ToString());
                                writer.WriteLine();

                                while (ex != null)
                                {
                                    writer.WriteLine(ex.GetType().FullName);
                                    writer.WriteLine("Message : " + ex.Message);
                                    writer.WriteLine("StackTrace : " + ex.StackTrace);

                                    ex = ex.InnerException;
                                }
                            }
                            MessageBox.Show("Error al crear la base de datos");
                        }
                    }


                }
                catch (Exception ex)
                {
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string filePath = path + @"\Error.txt";
                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.WriteLine("-----------------------------------------------------------------------------");
                        writer.WriteLine("Date : " + DateTime.Now.ToString());
                        writer.WriteLine();

                        while (ex != null)
                        {
                            writer.WriteLine(ex.GetType().FullName);
                            writer.WriteLine("Message : " + ex.Message);
                            writer.WriteLine("StackTrace : " + ex.StackTrace);

                            ex = ex.InnerException;
                        }
                    }
                    MessageBox.Show("Error al crear la conexion a la base de datos.\n Verifique que las credenciales proporcionadas sean las correctas y que la configuracion de Mysql sea la correcta para permitir conexiones externas"); 
                }

            }

        }

    }
}
