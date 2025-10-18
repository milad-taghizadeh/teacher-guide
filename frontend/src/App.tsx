import { Navigate, Route, Routes } from "react-router-dom";
import ProtectedRoute from "./components/ProtectedRoute";
import DashboardLayout from "./components/DashboardLayout";
import { AuthProvider } from "./providers/AuthProvider";
import LoginPage from "./pages/LoginPage";
import InvitesPage from "./pages/InvitesPage";
import SubmissionsPage from "./pages/SubmissionsPage";
import SubmissionFormPage from "./pages/SubmissionFormPage";

const App = () => {
  return (
    <AuthProvider>
      <Routes>
        <Route path="/" element={<Navigate to="/login" replace />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/form/:token" element={<SubmissionFormPage />} />
        <Route element={<ProtectedRoute />}>
          <Route path="/panel" element={<DashboardLayout />}>
            <Route index element={<Navigate to="invites" replace />} />
            <Route path="invites" element={<InvitesPage />} />
            <Route path="submissions" element={<SubmissionsPage />} />
          </Route>
        </Route>
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
    </AuthProvider>
  );
};

export default App;
