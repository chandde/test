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
import hamburger from './hamburger.png';
import bellevue from './bellevue.jpg';
import footer from './footer.png';
import './App.css';
import { useState } from 'react';
import { Button } from 'react-bootstrap';
import Modal from 'react-modal';

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
  bettyContact: [
    "Betty Dong - Managing Broker",
    "董娜 - 管理经纪人",
  ],
  haoContact: [
    "Hao Sun - Broker",
    "孙浩 - 经纪人",
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
  previousTransactions: [
    "Previous transactions",
    "历史交易记录"
  ],
  generalIntroduction: [
    "Welcome to Belhomes. We're Hao and Betty, two seasoned real estate agent currently at Compass Real Estate (R). We work full time and cover your needs 7x24, whenever you need to see a property, just give us a call. We provide service in the greater seattle area, including Seattle, Bellevue, Redmond, Issaquah, Sammamish, Kirkland and more."
  ],
  customerReview: [
    "Reviews",
    "用户评价"
  ],
  gallery: [
    "Gallery",
    "图集"
  ],
  pastTransactions: [
    "All from our past transactions",
    "全部来自我们的过往交易记录"
  ],
  contactUs: [
    "Contact us",
    "联系我们"
  ],
  backToTop: [
    "Back to top",
    "返回顶部"
  ],
};

const reviews = [
  {
    name: 'Sam He',
    date: '11/28/2019',
    desc: 'Bought and sold a home in 2019 in Cougar Mountain, Bellevue, WA 98006.',
    review: 'I have known Betty for couple of years now. She helped me a few years ago as a buyer agent and again this year as a seller agent. In both cases, I am very impressed with her professionalism and acute awareness of the real-estate markets. Selling my property wasn\'t an easy task due to a slow market and competitions from other similar prosperities on sale. But she and her partner was able to successfully sell and close the deal less than a week to a perfect buyer! I must be thankful for her expertise on timing, pricing, and negotiations skills. The staging service provided by her team was also amazing. My property looked gorgeous after transformation. Really nice job done throughout! I would definitely recommend Betty and her team if you are interested in buying/selling and I look forward to working with her again in the future.',
  },
  {
    name: 'Joyce 2011',
    date: '11/09/2019',
    desc: 'Bought a Single Family home in 2019 in Issaquah, WA.',
    review: 'Betty behaves so professional during the whole process of buying the new house. She is super responsive for our questions and concerns. Always giving impartial advice, respect our opinions and never pushing us to make immature decisions. With her hard work, we moved into our lovely new house after 4 months looking. Everything went smooth.',
  },
  {
    name: 'ocean00paradise',
    date: '10/31/2017',
    desc: 'Bought a Single Family home in 2017 in Kirkland, WA.',
    review: 'We spent almost an half year to work with Betty and finally found and bought our dream house , she exhibits the best quality of an agent which every buyer would dream of , and to be very honest is much beyond our expectations , She is extremely accountable, patient and responsive, always at call and quick in action, she really likes to spend time with us in every tours and repeatedly discuss with us to dig out our uncovered needs which we didn’t realize in the very beginning. Mostly importantly she is so insightful of local markets, and has an unique perspectives that really opens our mind and knows what kind of property we should buy, we value her opinions very much in our decision making. She is simply the best , better than any other agents that we have experienced and worked with, because she never pushes clients to buy the house , she help them to find the value of the property and deal . we strongly recommend you to work with Betty , she will be beyond your expectations.',
  },
  {
    name: 'Liangmin Zhou',
    date: '10/24/2017',
    desc: 'Bought a Single Family home in 2017 in Sammamish, WA.',
    review: 'We got acquainted with Betty occasionally on a friend\'s house closing day. Both my husband and I feel like she is reliable when we first met her. She has impressed us so much by her prompt reply to our questions of all kinds even during late night or weekends. She can always give you a more than satisfactory answer to our questions in housing, living, childcare and so on. As an agent, she is professional and enthusiastic in her job. She helped us going through the whole house-buying process including everything you need to do to buy a house. When we work with her, we didn\'t feel like she treats us as customers. Instead, we feel like we are more like friends or families. I would highly recommend her to anyone who plan to buy a house in great Seattle area. What she can do for you deserves your trust.',
  },
  {
    name: 'Ding Luo',
    date: '06/14/2017',
    desc: 'Bought a Single Family home in 2016 in Snoqualmie, WA.',
    review: 'Betty represented my wife and I during our real estate purchases. Betty easily approachable and always shows sincere care toward her clients. She knows exactly what her clients\' look for in their ideal home, utilizes her familiarity with the Greater Seattle area to deliver excellent housing sources to her client. During house visits, Betty is able to point out potential problems to her clients. When making purchase offers, not only she constructs a very strong offer, she keeps her client protected at same time. Aside from physical houses, Betty has good resources with other house-related needs. Whether it\'s house inspectors, cleaners, general contractors, loan agents, Betty only recommends the most skillful and most experience experts in each respective area. Betty is a precise communicator whom always responds very promptly. Betty sets the bar for the Seattle area agents and I wholeheartedly recommend Betty',
  },
  {
    name: 'Alex Man',
    date: '11/30/2017',
    desc: 'Bought a Townhouse home in 2017 in Seattle, WA.',
    review: 'Betty is very friendly, patience and very responsive. She\'s knowledgeable. She helped me to get my house below listing, then helped me to deal with some rental stuff. I highly recommend her.',
  },
  {
    name: 'user88117872',
    date: '11/02/2017',
    desc: 'Bought a Single Family home in 2017 in Seattle, WA.',
    review: 'Betty is definitely an All Star agent! She helped us close an investment property in red hot Seattle. We do not resident in Seattle so she was the representative to do all the works for us. Her negotiation skill is superb that she bought us a townhouse with less than listing price. She’s always responsive no matter how trivial our requests were. Definitely recommend if you are looking for someone who you can trust!',
  },
  {
    name: 'nepa3',
    date: '06/04/2017',
    desc: 'Bought and sold a Condo home in 2016 in Issaquah, WA.',
    review: 'Betty was my agent who help me sold my house at Seattle and also bought a very nice one at east side last year. we became friend during the months of discussion,house hunting and formalities finalization, Betty has passion about the work, and patient, but the most import quality in her is honest. I felt ease to work with her because she always listen what you want, do what she can best and never push you reluctant for her own good. Is it enough for a good agent? I think the answer is YES.',
  }
];

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

  const reviewItem = (item, active) => (<div className={`item ${active ? 'active' : ''}`}>
    <div className="reviewheader">
      <div className="name">{item.name}</div>
      <div className="date">{item.date}</div>
    </div>
    <div className="desc">{item.desc}</div>
    <br />
    <br />
    <div><i>{item.review}</i></div>
  </div>);

  return (
    <div className="App">
      <div className="navbar">
        <a className="navbaritem" href="#backtotopanchor">{getString("backToTop")}</a>
        <a className="navbaritem" href="#whoareweanchor">{getString("whoAreWeTitle")}</a>
        <a className="navbaritem" href="#reviewanchor">{getString("customerReview")}</a>
        <a className="navbaritem" href="#galleriesanchor">{getString("gallery")}</a>
        <a className="navbaritem" href="#contactusanchor">{getString("contactUs")}</a>
      </div>
      <div className="header section" id="backtotopanchor">
        <div className="headerwrapper">
          <div className="headerleft">
            <div className="headerlogowrapper">
              <img src={hamburger} className="hamburgerlogo" />
              <img src={logo} className="headerlogo" />
            </div>
            <h1 className="title">{getString("title")}</h1>
            <h2 className="subtitle">{getString("subtitle")}</h2>
          </div>
          <div className="headerright">
            <div className="headerrightinside">
              <Button variant="primary" onClick={switchLocale} className="langSwitch">
                {getString("langSwitch")}
              </Button>
              {/* <a href="#contactusanchor" className="contactuslink">{getString("contactUs")}</a> */}
            </div>
          </div>
        </div>
      </div>
      <div className="body" id="whoareweanchor">
        <h2>{getString("whoAreWeTitle")}</h2>
        <div className="separator"></div>
        <div className="generalIntroduction">
          {getString("generalIntroduction")}
        </div>
        <div className="introduction">
          {introduction("haoContact", "haoIntroduction", haoPic, "Hao Sun")}
          {introduction("bettyContact", "bettyIntroduction", bettyPic, "Betty Dong")}
        </div>
        {/* <h2>{getString("whatWeDoTitle")}</h2> */}
        <h2 id="reviewanchor">{getString("customerReview")}</h2>
        <div className="separator"></div>
        <div id="myCarousel" class="carousel reviewcarousel" data-ride="carousel">
          <ol class="carousel-indicators">
            <li data-target="#myCarousel" data-slide-to="0" class="active"></li>
            <li data-target="#myCarousel" data-slide-to="1"></li>
            <li data-target="#myCarousel" data-slide-to="2"></li>
            <li data-target="#myCarousel" data-slide-to="3"></li>
            <li data-target="#myCarousel" data-slide-to="4"></li>
            <li data-target="#myCarousel" data-slide-to="5"></li>
            <li data-target="#myCarousel" data-slide-to="6"></li>
            <li data-target="#myCarousel" data-slide-to="7"></li>
          </ol>
          <div class="carousel-inner">
            {reviewItem(reviews[0], true)}
            {reviewItem(reviews[1])}
            {reviewItem(reviews[2])}
            {reviewItem(reviews[3])}
            {reviewItem(reviews[4])}
            {reviewItem(reviews[5])}
            {reviewItem(reviews[6])}
            {reviewItem(reviews[7])}
          </div>
          <a class="left carousel-control" href="#myCarousel" data-slide="prev">
            <span class="glyphicon glyphicon-chevron-left"></span>
            <span class="sr-only">Previous</span>
          </a>
          <a class="right carousel-control" href="#myCarousel" data-slide="next">
            <span class="glyphicon glyphicon-chevron-right"></span>
            <span class="sr-only">Next</span>
          </a>
        </div>
        <h2 id="galleriesanchor">{getString("gallery")}</h2>
        <div className="separator"></div>
        <h3>{getString("pastTransactions")}</h3>
        <div id="myCarousel" class="carousel picturecarousel" data-ride="carousel">
          <ol class="carousel-indicators">
            <li data-target="#myCarousel" data-slide-to="0" class="active"></li>
            <li data-target="#myCarousel" data-slide-to="1"></li>
            <li data-target="#myCarousel" data-slide-to="2"></li>
            <li data-target="#myCarousel" data-slide-to="3"></li>
            <li data-target="#myCarousel" data-slide-to="4"></li>
            <li data-target="#myCarousel" data-slide-to="5"></li>
            <li data-target="#myCarousel" data-slide-to="6"></li>
          </ol>
          <div class="carousel-inner">
            <div class="item active">
              <img src={img1} />
            </div>
            <div class="item">
              <img src={img2} />
            </div>
            <div class="item">
              <img src={img3} />
            </div>
            <div class="item">
              <img src={img4} />
            </div>
            <div class="item">
              <img src={img6} />
            </div>
            <div class="item">
              <img src={img7} />
            </div>
            <div class="item">
              <img src={img8} />
            </div>
          </div>
          <a class="left carousel-control" href="#myCarousel" data-slide="prev">
            <span class="glyphicon glyphicon-chevron-left"></span>
            <span class="sr-only">Previous</span>
          </a>
          <a class="right carousel-control" href="#myCarousel" data-slide="next">
            <span class="glyphicon glyphicon-chevron-right"></span>
            <span class="sr-only">Next</span>
          </a>
        </div>
        <h2 id="contactusanchor">{getString("contactUs")}</h2>
        <div className="separator"></div>
        <div className="contactwrapper">
          <div className="contactTable">
            <h2>{getString("hao")}</h2>
            {contact("phonenumber", "(+1) 425.890.7988")}
            {contact("email", "hao_sun@hotmail.com")}
            {locale === LangId.Chinese && contact("wechat", "haohao667")}
            {/* {contact("website", "https://www.compass.com/agents/hao-sun/")} */}
          </div>
          <div className="contactTable">
            <h2>{getString("betty")}</h2>
            {contact("phonenumber", "(+1) 425.615.1552")}
            {contact("email", "bettydongpersonal@gmail.com")}
            {locale === LangId.Chinese && contact("wechat", "bettydongseattle")}
            {/* {contact("website", "https://www.compass.com/agents/betty-dong/")} */}
          </div>
        </div>
        <div className="footer">
          <div className="endmark">
            <div>&#167;</div>
            <div>&#167;</div>
            <div>&#167;</div>
          </div>
          <img src={footer} className="footerimage" />
          {/* <div className="separator"></div> */}
          {/* <div className="poweredby">@Powered by Chandler Deng</div> */}
        </div>
      </div>
    </div>
  );
}

export default App;
