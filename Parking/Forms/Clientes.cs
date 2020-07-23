using Newtonsoft.Json;
using Parking.Modelos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Parking
{
    public partial class Clientes : Form
    {


        #region Variables
        string rutaJson = Environment.CurrentDirectory + @"\data.json";
        string CadenaConexion;

        public delegate void RF();
        public event RF Refresh_;
        string Conexion;
        string Usuario;
        int ID_Cliente;
        #endregion
        public Clientes(string conexion, string usuario)
        {
            Conexion = conexion;
            Usuario = usuario;
            InitializeComponent();
            ModeloDatos oSettings = JsonConvert.DeserializeObject<ModeloDatos>(File.ReadAllText(rutaJson));
            CadenaConexion = oSettings.CadenaConexion;

        }

        public void SoloLetras(object sender, KeyPressEventArgs e)
        {

            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) &&
        (e.KeyChar != ' '))

            {
                e.Handled = true;
            }
        }


        private void btnNuevoCliente_Click(object sender, EventArgs e)
        {
            using (var a = new Forms.NuevaPension(Conexion, Usuario))
            {
                a.ShowDialog();

            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNombre.Text))
            {
                MessageBox.Show("Coloque un Nombre");
            }
            else
            {
                LlenarTabla("Nombre");
                btnAgregarPension.Visible = (dataGridView.Rows.Count>0)?true:false;
                btnEliminar.Visible = (dataGridView.Rows.Count > 0) ? true : false;

            }
        }
        private int? GetID()
        {
            try
            {
                var a = dataGridView.Rows[dataGridView.CurrentRow.Index].Cells[0].Value.ToString();
                ID_Cliente = Int32.Parse(a);
                return ID_Cliente;
            }
            catch
            {
                return null;

            }
        }

        private void LlenarTabla(string Busqueda)
        {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var listabusqueda = (from cl in context.cliente
                                     join vh in context.vehiculo on cl.Id_Vehiculo equals vh.Id_Vehiculo
                                     select new Modelos.ModeloBusquedaCliente
                                     {
                                         ID = cl.Id_cliente,
                                         Nombre = cl.Nombre+" "+cl.Apellido_paterno+" "+cl.Apellido_materno,
                                         Direccion= cl.Direccion,
                                         Tel1= cl.Tel1,
                                         CantidadPensiones= (from bp in context.banco_pension where(bp.Id_cliente==cl.Id_cliente)select bp.ID).Count(),
                                         RFC=cl.RFC,
                                         Placa=vh.Placa

                                     }).AsQueryable();
                switch (Busqueda)
                {
                    case "Nombre":
                        listabusqueda = listabusqueda.Where(f => f.Nombre.Contains(txtNombre.Text.Trim()));
                        dataGridView.DataSource = listabusqueda.ToList();
                        break;
                    case "Todo":
                         dataGridView.DataSource = listabusqueda.ToList();
                        break;
                }
            }
        }

        private void btnBuscarTodos_Click(object sender, EventArgs e)
        {
            LlenarTabla("Todo");
            btnAgregarPension.Visible = (dataGridView.Rows.Count > 0) ? true : false;
            btnEliminar.Visible = (dataGridView.Rows.Count > 0) ? true : false;
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int? id = GetID();
            if (id != null)
            {
                btnAgregarPension.Visible = true;
                btnEliminar.Visible = true;
            }
        }

        private void btnAgregarPension_Click(object sender, EventArgs e)
        {
            int? id = GetID();
            if (id != null)
            {
                using (var a = new Forms.AgregarPension(Conexion, Usuario,id.GetValueOrDefault()))
                {
                    a.ShowDialog();

                }
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            int? id = GetID();
            if (id != null)
            {
                DialogResult resultado = MessageBox.Show("¿Desea eliminar el Cliente con el id: " +  id.ToString() + "? \n Este Cambio es irreversible todos los datos relacionados al cliente serán eliminados", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (resultado == DialogResult.Yes)
                {
                    using (var context = new PARKING_DBEntities(CadenaConexion))
                    {
                        var a = context.banco_pension.Where(n => n.Id_cliente == id);
                        context.banco_pension.RemoveRange(a);

                        var b = context.cliente.Where(n => n.Id_cliente == id);                        
                        var tcliente = b.ToList().LastOrDefault();
                        var idvh = tcliente.Id_Vehiculo;
                        context.cliente.RemoveRange(b);


                        var c = context.vehiculo.Where(n => n.Id_Vehiculo ==idvh).FirstOrDefault();
                        context.vehiculo.Remove(c);


                        context.SaveChanges();
                    }                    
                    LlenarTabla("Todas");
                }
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
         
            this.Close();
        }

        private void Clientes_FormClosing(object sender, FormClosingEventArgs e)
        {
            Refresh_();
        }
    }
}
