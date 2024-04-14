import { Container, Row, Col } from "reactstrap";
import "../assets/css/LoginCss.css";
import { useLogin } from "../hooks/contexts";
import Logo from "../assets/img/logo.png";

const LoginPage = () => {
  const [creds, setLoginCreds, onSubmit] = useLogin();
  const { idnumber, key } = creds;
  return (
    <Container fluid className="login-page">
      <Row className="align-items-center justify-content-center">
        <Col lg={5} className="login-container">
          <div className="card">
            <Row>
              <Row className="align-items-center justify-content-center">
                <img className="login-logo" src={Logo} alt="" />
              </Row>
              <Row className="mt-3">
                <h2>LCCTO-LMS</h2>
              </Row>
            </Row>
            <form onSubmit={(e) => onSubmit(e)}>
              <input
                type="text"
                name="idnumber"
                id="idnumber"
                value={idnumber}
                onChange={(e) => setLoginCreds(e.target.name, e.target.value)}
                placeholder="Student Number"
                required
              />
              <input
                type="password"
                name="key"
                id="key"
                value={key}
                onChange={(e) => setLoginCreds(e.target.name, e.target.value)}
                placeholder="Password"
                required
              />
              <button type="submit">Login</button>
            </form>
          </div>
        </Col>
      </Row>
    </Container>
  );
};

export default LoginPage;
