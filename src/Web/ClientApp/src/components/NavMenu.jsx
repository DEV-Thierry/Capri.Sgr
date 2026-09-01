import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "./api-authorization/AuthContext";
import { ThemeToggle } from "./ThemeToggle";
function AuthLinks() { const { isAuthenticated, logout } = useAuth(); const navigate = useNavigate(); const leave = async event => { event.preventDefault(); await logout(); navigate("/"); }; return isAuthenticated ? <li><a href="#logout" onClick={leave}>Sair</a></li> : <><li><Link to="/login">Acessar</Link></li><li><Link to="/register">Associar-se</Link></li></>; }
export function NavMenu() { return <header><nav aria-label="Navegação principal"><ul><li><Link to="/">Capri.Sgr</Link></li></ul><ul><li><Link to="/">Início</Link></li><li><Link to="/publicacoes">Publicações</Link></li></ul><ul><AuthLinks /><li aria-hidden="true" className="nav-separator"/><li><ThemeToggle /></li></ul></nav></header>; }
