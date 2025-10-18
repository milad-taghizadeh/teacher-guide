import { useState } from "react";
import { useForm } from "react-hook-form";
import { apiClient } from "../api/client";

interface InviteFormValues {
  email: string;
  studentNumber: string;
  firstName?: string;
  lastName?: string;
}

interface InviteResponse {
  inviteId: string;
  token: string;
  expiresAt: string;
}

const InvitesPage = () => {
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting }
  } = useForm<InviteFormValues>();
  const [result, setResult] = useState<InviteResponse | null>(null);
  const [error, setError] = useState<string | null>(null);

  const onSubmit = async (data: InviteFormValues) => {
    setError(null);
    setResult(null);
    try {
      const response = await apiClient.post<InviteResponse>("/api/invites", data);
      setResult(response.data);
      reset();
    } catch (err) {
      setError("ارسال دعوت‌نامه با خطا مواجه شد");
    }
  };

  return (
    <div className="space-y-6">
      <div className="rounded-2xl bg-white p-6 shadow-sm">
        <h2 className="mb-4 text-lg font-semibold text-slate-800">ایجاد لینک ورود دانشجو</h2>
        <form className="grid grid-cols-1 gap-5 md:grid-cols-2" onSubmit={handleSubmit(onSubmit)}>
          <div className="md:col-span-1">
            <label className="mb-2 block text-sm font-medium text-slate-700">ایمیل دانشجو</label>
            <input
              type="email"
              className="w-full rounded border border-slate-200 px-3 py-2 focus:border-primary focus:outline-none"
              {...register("email", { required: "وارد کردن ایمیل الزامی است" })}
            />
            {errors.email && <p className="mt-1 text-xs text-red-500">{errors.email.message}</p>}
          </div>
          <div className="md:col-span-1">
            <label className="mb-2 block text-sm font-medium text-slate-700">شماره دانشجویی</label>
            <input
              type="text"
              className="w-full rounded border border-slate-200 px-3 py-2 focus:border-primary focus:outline-none"
              {...register("studentNumber", { required: "شماره دانشجویی را وارد کنید" })}
            />
            {errors.studentNumber && <p className="mt-1 text-xs text-red-500">{errors.studentNumber.message}</p>}
          </div>
          <div>
            <label className="mb-2 block text-sm font-medium text-slate-700">نام</label>
            <input
              type="text"
              className="w-full rounded border border-slate-200 px-3 py-2 focus:border-primary focus:outline-none"
              {...register("firstName")}
            />
          </div>
          <div>
            <label className="mb-2 block text-sm font-medium text-slate-700">نام خانوادگی</label>
            <input
              type="text"
              className="w-full rounded border border-slate-200 px-3 py-2 focus:border-primary focus:outline-none"
              {...register("lastName")}
            />
          </div>
          <div className="md:col-span-2 flex justify-end">
            <button
              type="submit"
              disabled={isSubmitting}
              className="rounded bg-primary px-6 py-2 text-white transition hover:bg-blue-600 disabled:opacity-60"
            >
              ارسال لینک
            </button>
          </div>
        </form>
        {error && <p className="mt-4 text-sm text-red-500">{error}</p>}
      </div>
      {result && (
        <div className="rounded-2xl bg-white p-6 shadow-sm">
          <h3 className="mb-3 text-base font-semibold text-slate-800">لینک ارسال شده</h3>
          <p className="text-sm text-slate-600">زمان انقضا: {new Date(result.expiresAt).toLocaleString("fa-IR")}</p>
          <div className="mt-3 rounded border border-dashed border-primary bg-blue-50 p-4 text-sm text-primary">
            {`${window.location.origin}/form/${result.token}`}
          </div>
        </div>
      )}
    </div>
  );
};

export default InvitesPage;
