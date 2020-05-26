const mysql = require('mysql');

exports.Database = class Database {
  constructor() {
    console.log('Database ctor');
    this.connection = mysql.createConnection({
      host: 'localhost',
      user: 'codeshare1',
      password: '123456',
      database: 'codeshare'
    });
  }
  
  get(key, found, notFound) {
    console.log(`get mysql ${key}`);
    const start = new Date();
    this.connection.query(`SELECT * FROM codeshare WHERE id = '${key}'`,
    (error, results, fields) => {
      console.log(`took ${new Date() - start}ms to get mysql`);
      if(error || (results && !results[0])) {
        // if sql server returns error or empty output
        notFound();
      }
      if(results && results[0]) {
        found(results[0].data);
      }
    });
  }

  set(key, value) {
    console.log(`set mysql ${key}: ${value}`);
    const timeStamp = new Date().toJSON().slice(0, 19).replace('T', ' ');
    const start = new Date();
    this.connection.query(`
      INSERT INTO codeshare (id, data, lastAccessTime)
      VALUES ('${key}', '${value}', '${timeStamp}') 
      ON DUPLICATE KEY UPDATE data = '${value}', lastAccessTime = '${timeStamp}';`, (error, results, fields) => {
      console.log(`took ${new Date() - start}ms to set mysql`);
      // to do: update table failure
    });
  }
}
