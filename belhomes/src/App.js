import haoPic from './hao.jpg';
import bettyPic from './betty.jpg';
import logo from './logo.png';
import img1 from './tran1.jpg';
import img2 from './tran2.jpg';
import img3 from './tran3.jpg';
import img4 from './tran4.jpg';
import img6 from './tran6.jpg';
import img7 from './tran7.jpg';
import img8 from './tran8.jpg';
import bellevue from './bellevue.jpg';
import footer from './footer.png';
import './App.css';
import { useState } from 'react';
import { Button } from 'react-bootstrap';

const LangId = {
  English: 0,
  Chinese: 1,
};

const i18nResources = {
  title: [
    "Welcome to BelHomes!",
    "欢迎光临BelHomes！"
  ],
  subtitle: [
    "We're here to help! "
  ],
  langSwitch: [
    "中文", // show this on English page
    "English" // vice versa
  ],
  whoAreWeTitle: [
    "Who are we?",
    "我们是谁？"
  ],
  betty: [
    "Betty Dong",
    "董娜",
  ],
  hao: [
    "Hao Sun",
    "孙浩",
  ],
  haoIntroduction: [
    "Started as a real estate agent since 2016...",
    ""
  ],
  bettyIntroduction: [
    "I'm a Managing Real Estate Broker. I started since 2014. Fluent at English and Mandarin.",
    ""
  ],
  whatWeDoTitle: [
    "Services we provide",
    "",
  ],
  contactUsTitle: [
    "Contact us",
    "联系我们",
  ],
  previousTransactions: [
    "Previous transactions",
    "历史交易记录"
  ],
  generalIntroduction: [
    "Welcome to Belhomes. We're Hao and Betty, two seasoned real estate agent currently at Compass Real Estate (R). We work full time and cover your needs 7x24, whenever you need to see a property, just give us a call. We provide service in the greater seattle area, including Seattle, Bellevue, Redmond, Issaquah, Sammamish, Kirkland and more."
  ]
};

function App() {  
  const getString = (name) => i18nResources[name][locale] || "missing content";

  const [locale, setLocale] = useState(LangId.English);

  const switchLocale = () => {
    setLocale(locale === LangId.English ? LangId.Chinese : LangId.English)
  }

  const introduction = (name, introduction, picture, altText) => (
    <div>
      <h3>
        {getString(name)}
      </h3>
      <tr className="realtorTr">
        <th><div className="realtorPic"><img src={picture} alt={altText} /></div></th>
        <th><div className="realtorIntroduction">
          {getString(introduction)}
        </div></th>
      </tr>
    </div>
  );

  const contact = (className, data) => (
    <div className={`contact-item ${className}`}>
      {data}
    </div>
  );

  return (
    <div className="App">
      <div className="header">
        <div className="headerwrapper">
          <div className="headerleft">
            <div className="headerlogowrapper">
              <img src={logo} className="headerlogo"/>
            </div>
            <h1 className="title">{getString("title")}</h1>
            <h2 className="subtitle">{getString("subtitle")}</h2>
          </div>
          <div className="headerright">
            <div className="headerrightinside">
            <Button variant="primary" onClick={switchLocale} className="langSwitch">
              {getString("langSwitch")}
            </Button>
            <a href="#contactusanchor" className="contactuslink">Contact us</a>
            </div>   
          </div>
        </div>
      </div>
      <div className="gallery">
        {/* <img src={img2} /> */}
        {/* <img src={img3} />
        <img src={img4} />
        <img src={img1} />
        <img src={img6} />
        <img src={img7} />
        <img src={img8} /> */}
      </div>
      <div className="body">
        <h2>{getString("whoAreWeTitle")}</h2>
        <div className="generalIntroduction">
          {getString("generalIntroduction")}
        </div>
        <div className="introduction">
          {introduction("hao", "haoIntroduction", haoPic, "Hao Sun")}
          {introduction("betty", "bettyIntroduction", bettyPic, "Betty Dong")}
        </div>
        <h2>{getString("whatWeDoTitle")}</h2>
        <h2 id="contactusanchor">{getString("contactUsTitle")}</h2>
        <div className="contactwrapper">
          <div className="contactTable">
            <h2>{getString("hao")}</h2>
            {contact("phonenumber", "(+1) 425.890.7988")}
            {contact("email", "hao_sun@hotmail.com")}
            {contact("wechat", "haohao667")}
            {/* {contact("website", "https://www.compass.com/agents/hao-sun/")} */}
          </div>
          <div className="contactTable">
            <h2>{getString("betty")}</h2>
            {contact("phonenumber", "(+1) 425.615.1552")}
            {contact("email", "bettydongpersonal@gmail.com")}
            {contact("wechat", "bettydongseattle")}
            {/* {contact("website", "https://www.compass.com/agents/betty-dong/")} */}
          </div>
        </div>
        <div className="footer">
          <img src={footer} className="footerimage"/>
        </div>
      </div>
    </div>
  );
}

export default App;
