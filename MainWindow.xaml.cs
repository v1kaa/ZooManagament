using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;
using System.Data;

namespace WpfApp5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection sqlConnection;
        public MainWindow()
        {
            InitializeComponent();
            string connect = ConfigurationManager.ConnectionStrings["WpfApp5.Properties.Settings.new1ConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(connect);
            show_zoo();
            all_animals();
        }
        private void show_zoo()
        {
            string q = "select * from Zoo";
            SqlDataAdapter adapter = new SqlDataAdapter(q, sqlConnection);

            using (adapter)
            {
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                LIST_ZOO.DisplayMemberPath = "location";
                LIST_ZOO.SelectedValuePath = "Id";
                LIST_ZOO.ItemsSource = dataTable.DefaultView;
            }

        }

        private void LIST_ZOO_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            show_animals();
            show_selected_zoo_text();
        }
        private void show_animals()
        {


            try
            {
                string q = "select * from Animal inner join zooAnimal on Animal.Id = zooAnimal.animal_id where zooAnimal.zoo_id =@zoo_id";
                SqlCommand comandSql = new SqlCommand(q, sqlConnection);
                SqlDataAdapter adapter = new SqlDataAdapter(comandSql);

                using (adapter)
                {
                    comandSql.Parameters.AddWithValue("@zoo_id", LIST_ZOO.SelectedValue);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    animal_zoo_list.SelectedValuePath = "Id";
                    animal_zoo_list.DisplayMemberPath = "animal";
                    animal_zoo_list.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception)
            {


            }


        }
        private void all_animals()
        {
            
            string q = "select * from Animal";
            SqlDataAdapter adapter = new SqlDataAdapter(q, sqlConnection);
            using (adapter)
            {
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                animals_list.SelectedValuePath = "Id";
                animals_list.DisplayMemberPath = "animal";
                animals_list.ItemsSource = dataTable.DefaultView;
               
            }
          

        }

        private void Delete_zoo_Click(object sender, RoutedEventArgs e)
        {
            string query = "delete from Zoo where Id=@ZooId";
            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
            sqlConnection.Open();
            sqlCommand.Parameters.AddWithValue("@ZooId", LIST_ZOO.SelectedValue);
            sqlCommand.ExecuteScalar();
            sqlConnection.Close();
            show_zoo();

        }

        private void add_zoo_Click(object sender, RoutedEventArgs e)
        {
            string query = "insert into Zoo values (@location)";
            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
            sqlConnection.Open();
            sqlCommand.Parameters.AddWithValue("@location", text.Text);
            sqlCommand.ExecuteScalar();
            sqlConnection.Close();
            show_zoo();
        }
        private void add_animal_to_zoo(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "INSERT INTO zooAnimal (zoo_id, animal_id) VALUES (@zoo_id, @animal_id)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@zoo_id", LIST_ZOO.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@animal_id", animals_list.SelectedValue);
                sqlCommand.ExecuteNonQuery(); // Use ExecuteNonQuery since you're not expecting a return value
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log it, display an error message)
                MessageBox.Show("An error occurred: " + ex.Message);
            }
            finally
            {
                if (sqlConnection.State != ConnectionState.Closed)
                    sqlConnection.Close();
                show_animals();
            }
        }
        private void remove_animal_from_zoo(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from zooAnimal where zoo_id = @zoo_id and animal_id=@animal_id";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@zoo_id", LIST_ZOO.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@animal_id", animal_zoo_list.SelectedValue);
                sqlCommand.ExecuteNonQuery(); // Use ExecuteNonQuery since you're not expecting a return value
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log it, display an error message)
                MessageBox.Show("An error occurred: " + ex.Message);
            }
            finally
            {
                if (sqlConnection.State != ConnectionState.Closed)
                    sqlConnection.Close();
                show_animals();
            }
        }

        private void add_animal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into Animal values (@animal)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@animal", text.Text);
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                MessageBox.Show("An error occurred: " + ex.Message);
            }
            finally
            {
                if (sqlConnection.State != ConnectionState.Closed)
                    sqlConnection.Close();
                all_animals();
            }
        }
        private void delete_animal(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from Animal where Id= @animal_id";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@animal_id", animals_list.SelectedValue);
                sqlCommand.ExecuteNonQuery();

            }
            catch (Exception ex)
            {

                MessageBox.Show("An error occurred: " + ex.Message);
            }
            finally
            {

                sqlConnection.Close();
                all_animals();
            }
        }

        private void show_selected_zoo_text()
        {
            try
            {
                string q = "select location from Zoo where Id = @zoo_id";
                SqlCommand comandSql = new SqlCommand(q, sqlConnection);
                SqlDataAdapter adapter = new SqlDataAdapter(comandSql);

                using (adapter)
                {
                    comandSql.Parameters.AddWithValue("@zoo_id", LIST_ZOO.SelectedValue);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    text.Text = dataTable.Rows[0]["Location"].ToString();
                }
            }
            catch (Exception)
            {


            }
        }
        private void show_selected_anima_text()
        {
            try
            {
                string q = "select animal from Animal where Id = @animal_id";
                SqlCommand comandSql = new SqlCommand(q, sqlConnection);
                SqlDataAdapter adapter = new SqlDataAdapter(comandSql);

                using (adapter)
                {
                    comandSql.Parameters.AddWithValue("@animal_id", animals_list.SelectedValue);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    text.Text = dataTable.Rows[0]["animal"].ToString();
                }
            }
            catch (Exception)
            {


            }
        }

        private void animals_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            show_selected_anima_text();
        }
        public void update_zoo(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "update Zoo Set location = @Location where Id=@zoo_id";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@zoo_id", LIST_ZOO.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@location", text.Text);
                sqlCommand.ExecuteNonQuery(); // Use ExecuteNonQuery since you're not expecting a return value
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log it, display an error message)
                MessageBox.Show("An error occurred: " + ex.Message);
            }
            finally
            {
                if (sqlConnection.State != ConnectionState.Closed)
                    sqlConnection.Close();
                show_zoo();
            }
        }
        public void update_animal(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "update Animal Set animal = @animal where Id=@animal_id";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@animal_id", animals_list.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@animal", text.Text);
                sqlCommand.ExecuteNonQuery(); // Use ExecuteNonQuery since you're not expecting a return value
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log it, display an error message)
                MessageBox.Show("An error occurred: " + ex.Message);
            }
            finally
            {
                if (sqlConnection.State != ConnectionState.Closed)
                    sqlConnection.Close();
                   all_animals();
            }
        }

    }
}