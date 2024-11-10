
using FRC_App.Models;

//First (highest) level of the FRC data structure:
//i.e. holds all sessions of data
public class DataContainer {
    private User user { get; set; }
    private List<Session> sessions { get; set; }

    public DataContainer(User user) {
        this.user = user;
        this.sessions = new List<Session>{};

        List<string> sessions = user.sessions.Split("|",StringSplitOptions.RemoveEmptyEntries).ToList();
        foreach (string sessionName in sessions) {
            Session session = new Session();
            session.Name = sessionName;
            session.DataTypes = new List<DataType>{};

            DataImport dataStructure = new DataImport();
            List<List<List<double>>> rawData = dataStructure.RetrieveRawData(user,sessionName);

            int fileNum = 0;
            foreach (List<List<double>> type in rawData)
            {
                DataType dataType = new DataType();
                dataType.Name = dataStructure.dataTypes[fileNum];   
                dataType.Columns = new List<Column>{};

                int columnNum = 0;
                foreach (List<double> columnData in type)
                {
                    Column column = new Column();
                    column.Label = dataStructure.dataUnits[fileNum][columnNum];
                    column.Data = columnData;

                    dataType.Columns.Add(column);
                    columnNum++;
                }

                session.DataTypes.Add(dataType);
                fileNum++;
            }
            this.sessions.Add(session);
        }
    }


    public void storeDataContainer() {
        //TODO
    }


    /**
     * --- getSessionNames() ---
     * Returns the list of session names (testing, FRC Competition #2...). This should
     * be used to generate the buttons for the first selection criteria for
     * getting a data axis for plotting ect.
     * @return List<string>
     */
    public List<string> getSessionNames() {
        List<string> sessionNames = new List<string>{};

        foreach (Session session in this.sessions)
        {
            sessionNames.Add(session.Name);
        }

        return sessionNames;
    }


    /**
     * --- getSession() ---
     * Returns the Session object that corresponds to the sessionName. For example
     * after the session selection dropdown is chosen 
     * this would convert that string into the corresponding Session object that holds 
     * the dataTypes that hold columns with data & labels.
     * @param sessionName
     * @return Session
     */
    public Session getSession(string sessionName) {
        foreach (Session session in this.sessions)
        {
            if (String.Equals(session.Name,sessionName)) {
                return session;
            }
        }
        return null;
    }
}