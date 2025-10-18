import { NavLink, Outlet } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";

const DashboardLayout = () => {
  const { username, logout } = useAuth();

  return (
    <div className="min-h-screen bg-slate-100 text-slate-800">
      <header className="bg-white shadow-sm">
        <div className="mx-auto flex max-w-6xl items-center justify-between px-6 py-4">
          <div>
            <h1 className="text-xl font-semibold text-primary">سامانه انتخاب استاد</h1>
            <p className="text-sm text-slate-500">مدیریت دعوت‌نامه‌ها و فرم‌های دانشجویان</p>
          </div>
          <div className="flex items-center gap-4">
            <span className="text-sm">{username}</span>
            <button className="rounded bg-primary px-4 py-2 text-sm font-medium text-white" onClick={logout}>
              خروج
            </button>
          </div>
        </div>
      </header>
      <div className="mx-auto flex max-w-6xl gap-8 px-6 py-8">
        <nav className="w-56 space-y-2">
          <NavLink
            to="/panel/invites"
            className={({ isActive }) =>
              `block rounded px-4 py-3 text-sm font-medium ${isActive ? "bg-primary text-white" : "bg-white text-slate-700"}`
            }
          >
            ارسال دعوت‌نامه
          </NavLink>
          <NavLink
            to="/panel/submissions"
            className={({ isActive }) =>
              `block rounded px-4 py-3 text-sm font-medium ${isActive ? "bg-primary text-white" : "bg-white text-slate-700"}`
            }
          >
            فرم‌های دریافتی
          </NavLink>
        </nav>
        <main className="flex-1">
          <Outlet />
        </main>
      </div>
    </div>
  );
};

export default DashboardLayout;
