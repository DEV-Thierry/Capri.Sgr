import { Home } from "./components/Home";
import { Publications, PublicationDetail } from "./components/Publications";
import { LoginPage } from "./components/api-authorization/LoginPage";
import { RegisterPage } from "./components/api-authorization/RegisterPage";
const AppRoutes = [
  { index: true, element: <Home /> },
  { path: "/publicacoes", element: <Publications /> },
  { path: "/publicacoes/:id", element: <PublicationDetail /> },
  { path: "/login", element: <LoginPage /> },
  { path: "/register", element: <RegisterPage /> }
];
export default AppRoutes;
