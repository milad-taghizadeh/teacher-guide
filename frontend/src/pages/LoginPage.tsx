import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";

interface LoginFormValues {
  username: string;
  password: string;
}

const LoginPage = () => {
  const { login, token } = useAuth();
  const navigate = useNavigate();
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting }
  } = useForm<LoginFormValues>();
  const [formError, setFormError] = useState<string | null>(null);

  useEffect(() => {
    if (token) {
      navigate("/panel/invites");
    }
  }, [token, navigate]);

  const onSubmit = async (data: LoginFormValues) => {
    setFormError(null);
    try {
      await login(data.username, data.password);
      navigate("/panel/invites");
    } catch (error) {
      setFormError("نام کاربری یا رمز عبور نادرست است");
    }
  };

  return (
    <div className="flex min-h-screen items-center justify-center bg-slate-100 px-4">
      <div className="w-full max-w-md rounded-2xl bg-white p-8 shadow-lg">
        <h2 className="mb-6 text-center text-2xl font-semibold text-slate-800">ورود مدیر سامانه</h2>
        <form className="space-y-5" onSubmit={handleSubmit(onSubmit)}>
          <div>
            <label className="mb-2 block text-sm font-medium text-slate-700">نام کاربری</label>
            <input
              type="text"
              className="w-full rounded border border-slate-200 px-3 py-2 focus:border-primary focus:outline-none"
              {...register("username", { required: "وارد کردن نام کاربری الزامی است" })}
            />
            {errors.username && <p className="mt-1 text-xs text-red-500">{errors.username.message}</p>}
          </div>
          <div>
            <label className="mb-2 block text-sm font-medium text-slate-700">رمز عبور</label>
            <input
              type="password"
              className="w-full rounded border border-slate-200 px-3 py-2 focus:border-primary focus:outline-none"
              {...register("password", { required: "وارد کردن رمز عبور الزامی است" })}
            />
            {errors.password && <p className="mt-1 text-xs text-red-500">{errors.password.message}</p>}
          </div>
          {formError && <p className="text-center text-sm text-red-500">{formError}</p>}
          <button
            type="submit"
            disabled={isSubmitting}
            className="w-full rounded bg-primary py-2 text-white transition hover:bg-blue-600 disabled:opacity-60"
          >
            ورود
          </button>
        </form>
      </div>
    </div>
  );
};

export default LoginPage;
