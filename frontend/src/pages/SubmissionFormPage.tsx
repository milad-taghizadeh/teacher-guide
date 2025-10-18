import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useParams } from "react-router-dom";
import { apiClient } from "../api/client";

interface InviteDetails {
  inviteId: string;
  email: string;
  studentNumber: string;
  firstName?: string | null;
  lastName?: string | null;
  expiresAt: string;
  isUsed: boolean;
}

interface SubmissionFormValues {
  studentNumber: string;
  firstName: string;
  lastName: string;
  teacherName: string;
  phoneNumber: string;
  email: string;
  topicTitle: string;
  description: string;
}

const SubmissionFormPage = () => {
  const { token } = useParams<{ token: string }>();
  const [invite, setInvite] = useState<InviteDetails | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [isSubmitted, setIsSubmitted] = useState<boolean>(false);
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting }
  } = useForm<SubmissionFormValues>();

  useEffect(() => {
    const fetchInvite = async () => {
      try {
        const response = await apiClient.get<InviteDetails>(`/api/invites/${token}`);
        setInvite(response.data);
        reset({
          studentNumber: response.data.studentNumber,
          email: response.data.email
        });
      } catch (err: any) {
        if (err.response?.status === 410) {
          setError("این لینک منقضی شده است");
        } else {
          setError("لینک معتبر نیست");
        }
      } finally {
        setIsLoading(false);
      }
    };

    if (token) {
      fetchInvite();
    }
  }, [token, reset]);

  const onSubmit = async (data: SubmissionFormValues) => {
    if (!token) {
      return;
    }
    setError(null);
    try {
      await apiClient.post("/api/submissions", {
        token,
        ...data
      });
      setIsSubmitted(true);
    } catch (err: any) {
      if (err.response?.status === 410) {
        setError("این لینک قبلا استفاده شده یا منقضی شده است");
      } else {
        setError("ارسال فرم با مشکل روبرو شد");
      }
    }
  };

  if (isLoading) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-slate-100 px-4">
        <div className="rounded-2xl bg-white px-10 py-12 text-slate-600 shadow-sm">در حال بررسی لینک...</div>
      </div>
    );
  }

  if (error || !invite) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-slate-100 px-4">
        <div className="rounded-2xl bg-white px-10 py-12 text-center text-slate-700 shadow-sm">{error ?? "لینک معتبر نیست"}</div>
      </div>
    );
  }

  if (isSubmitted) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-slate-100 px-4">
        <div className="max-w-lg rounded-2xl bg-white px-10 py-12 text-center text-slate-700 shadow-sm">
          <h2 className="mb-3 text-2xl font-semibold text-primary">اطلاعات با موفقیت ثبت شد</h2>
          <p className="text-sm text-slate-600">از همکاری شما سپاسگزاریم. نتیجه بررسی از طریق ایمیل به اطلاع خواهد رسید.</p>
        </div>
      </div>
    );
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-slate-100 px-4">
      <div className="w-full max-w-3xl rounded-2xl bg-white p-8 shadow-lg">
        <h2 className="mb-4 text-2xl font-semibold text-slate-800">فرم درخواست راهنمایی</h2>
        {(() => {
          const fullName = [invite.firstName, invite.lastName].filter(Boolean).join(" ");
          return fullName ? (
            <p className="text-sm text-slate-600">{fullName} عزیز، لطفا اطلاعات زیر را تکمیل کنید.</p>
          ) : null;
        })()}
        <p className="mb-6 mt-2 text-sm text-slate-600">پس از ارسال فرم امکان ویرایش وجود نخواهد داشت.</p>
        <form className="grid grid-cols-1 gap-5 md:grid-cols-2" onSubmit={handleSubmit(onSubmit)}>
          <div>
            <label className="mb-2 block text-sm text-slate-700">شماره دانشجویی</label>
            <input
              type="text"
              className="w-full rounded border border-slate-200 px-3 py-2 focus:border-primary focus:outline-none"
              {...register("studentNumber", { required: "شماره دانشجویی را وارد کنید" })}
            />
            {errors.studentNumber && <p className="mt-1 text-xs text-red-500">{errors.studentNumber.message}</p>}
          </div>
          <div>
            <label className="mb-2 block text-sm text-slate-700">نام</label>
            <input
              type="text"
              className="w-full rounded border border-slate-200 px-3 py-2 focus:border-primary focus:outline-none"
              {...register("firstName", { required: "نام را وارد کنید" })}
            />
            {errors.firstName && <p className="mt-1 text-xs text-red-500">{errors.firstName.message}</p>}
          </div>
          <div>
            <label className="mb-2 block text-sm text-slate-700">نام خانوادگی</label>
            <input
              type="text"
              className="w-full rounded border border-slate-200 px-3 py-2 focus:border-primary focus:outline-none"
              {...register("lastName", { required: "نام خانوادگی را وارد کنید" })}
            />
            {errors.lastName && <p className="mt-1 text-xs text-red-500">{errors.lastName.message}</p>}
          </div>
          <div>
            <label className="mb-2 block text-sm text-slate-700">نام استاد پیشنهادی</label>
            <input
              type="text"
              className="w-full rounded border border-slate-200 px-3 py-2 focus:border-primary focus:outline-none"
              {...register("teacherName", { required: "نام استاد را وارد کنید" })}
            />
            {errors.teacherName && <p className="mt-1 text-xs text-red-500">{errors.teacherName.message}</p>}
          </div>
          <div>
            <label className="mb-2 block text-sm text-slate-700">شماره تماس</label>
            <input
              type="tel"
              className="w-full rounded border border-slate-200 px-3 py-2 focus:border-primary focus:outline-none"
              {...register("phoneNumber", { required: "شماره تماس را وارد کنید" })}
            />
            {errors.phoneNumber && <p className="mt-1 text-xs text-red-500">{errors.phoneNumber.message}</p>}
          </div>
          <div>
            <label className="mb-2 block text-sm text-slate-700">ایمیل</label>
            <input
              type="email"
              className="w-full rounded border border-slate-200 px-3 py-2 focus:border-primary focus:outline-none"
              {...register("email", { required: "ایمیل را وارد کنید" })}
            />
            {errors.email && <p className="mt-1 text-xs text-red-500">{errors.email.message}</p>}
          </div>
          <div className="md:col-span-2">
            <label className="mb-2 block text-sm text-slate-700">عنوان موضوع پیشنهادی</label>
            <input
              type="text"
              className="w-full rounded border border-slate-200 px-3 py-2 focus:border-primary focus:outline-none"
              {...register("topicTitle", { required: "عنوان پژوهش را وارد کنید" })}
            />
            {errors.topicTitle && <p className="mt-1 text-xs text-red-500">{errors.topicTitle.message}</p>}
          </div>
          <div className="md:col-span-2">
            <label className="mb-2 block text-sm text-slate-700">توضیحات تکمیلی</label>
            <textarea
              rows={4}
              className="w-full rounded border border-slate-200 px-3 py-2 focus:border-primary focus:outline-none"
              {...register("description", { required: "توضیحات را تکمیل کنید" })}
            />
            {errors.description && <p className="mt-1 text-xs text-red-500">{errors.description.message}</p>}
          </div>
          {error && (
            <div className="md:col-span-2 text-sm text-red-500">{error}</div>
          )}
          <div className="md:col-span-2 flex justify-end">
            <button
              type="submit"
              disabled={isSubmitting}
              className="rounded bg-primary px-6 py-2 text-white transition hover:bg-blue-600 disabled:opacity-60"
            >
              ارسال فرم
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default SubmissionFormPage;
