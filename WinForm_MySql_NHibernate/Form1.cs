using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using NHibernate;
using NHibernate.Cfg;

namespace WinForm_MySql_NHibernate
{
    public partial class Form1 : Form
    {
        ISession session;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                Configuration config = new Configuration();
                config.Configure(@"Mapping\Student.cfg.xml");

                ISessionFactory factory = config.BuildSessionFactory();
                session = factory.OpenSession();
            }
            catch (Exception ex)
            {
                if (session != null) session.Close();
                MessageBox.Show(ex.Message, "NHibernate Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void Read_Click(object sender, EventArgs e)
        {
            ICriteria criteria = session.CreateCriteria(typeof(Student));
            List<Student> list = new List<Student>();

            criteria.List(list);

            dataGridView1.Rows.Clear();

            for (int i = 0; i < list.Count; i++)
            {
                object[] arr = { list[i].grade, list[i].cclass, list[i].no, list[i].name, list[i].score };
                dataGridView1.Rows.Add(arr);
            }
        }

        private void Create_Click(object sender, EventArgs e)
        {
            using (session.BeginTransaction())
            {
                Student student = GetRowStudent();

                session.Save(student);
                session.Flush();
                session.Transaction.Commit();

                Read.PerformClick();
            }
        }

        private void Update_Click(object sender, EventArgs e)
        {
            using (session.BeginTransaction())
            {
                Student student = GetRowStudent();

                session.Merge(student);
                session.Flush();
                session.Transaction.Commit();

                Read.PerformClick();
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            using (session.BeginTransaction())
            {
                Student student = session.Get<Student>((int)dataGridView1.SelectedCells[0].OwningRow.Cells["no"].Value);

                session.Delete(student);
                session.Flush();
                session.Transaction.Commit();

                Read.PerformClick();
            }
        }

        private Student GetRowStudent()
        {
            Student std = new Student
            {
                grade = System.Convert.ToInt32(dataGridView1.SelectedCells[0].OwningRow.Cells["grade"].Value),
                cclass = System.Convert.ToInt32(dataGridView1.SelectedCells[0].OwningRow.Cells["cclass"].Value),
                no = System.Convert.ToInt32(dataGridView1.SelectedCells[0].OwningRow.Cells["no"].Value),
                name = (string)dataGridView1.SelectedCells[0].OwningRow.Cells["name"].Value,
                score = (string)dataGridView1.SelectedCells[0].OwningRow.Cells["score"].Value
            };

            return std;
        }
    }
}
