import { Container, Row, Col } from "reactstrap";
import "../assets/css/LoginCss.css";
import { useLogin, useGetLoggedUser } from "../hooks/contexts";
import Logo from "../assets/img/logo.png";
import { useNavigate } from "react-router-dom";
import { toast } from "react-toastify";

const LoginPage = () => {
  const [creds, setLoginCreds, onSubmit] = useLogin();
  const navigate = useNavigate();

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
            <form
              onSubmit={async (e) => {
                if (await onSubmit(e)) {
                  navigate("/home");
                  toast.success("Login successful");
                } else {
                  toast.error("Wrong password or student number");
                }
              }}
            >
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
