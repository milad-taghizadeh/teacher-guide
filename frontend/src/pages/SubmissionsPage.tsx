import { useMemo, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import dayjs from "dayjs";
import { apiClient } from "../api/client";
import { SubmissionStatus } from "../utils/status";

interface SubmissionSummary {
  submissionId: string;
  studentNumber: string;
  fullName: string;
  teacherName: string;
  topicTitle: string;
  status: SubmissionStatus;
  submittedAt: string;
}

const statusLabels: Record<SubmissionStatus, string> = {
  Pending: "در انتظار بررسی",
  UnderReview: "در حال بررسی",
  Approved: "تایید شده",
  Rejected: "رد شده"
};

const statusOptions = [
  { value: "", label: "همه وضعیت‌ها" },
  { value: "Pending", label: statusLabels.Pending },
  { value: "UnderReview", label: statusLabels.UnderReview },
  { value: "Approved", label: statusLabels.Approved },
  { value: "Rejected", label: statusLabels.Rejected }
];

const SubmissionsPage = () => {
  const [status, setStatus] = useState<string>("");
  const [search, setSearch] = useState<string>("");
  const [from, setFrom] = useState<string>("");
  const [to, setTo] = useState<string>("");
  const queryClient = useQueryClient();

  const queryParams = useMemo(() => ({ status, search, from, to }), [status, search, from, to]);

  const { data, isLoading, isError } = useQuery({
    queryKey: ["submissions", queryParams],
    queryFn: async () => {
      const response = await apiClient.get<SubmissionSummary[]>("/api/submissions", {
        params: {
          status: status || undefined,
          search: search || undefined,
          from: from ? dayjs(from).toISOString() : undefined,
          to: to ? dayjs(to).endOf("day").toISOString() : undefined
        }
      });
      return response.data;
    }
  });

  const updateStatusMutation = useMutation({
    mutationFn: async ({ id, newStatus }: { id: string; newStatus: SubmissionStatus }) => {
      await apiClient.patch(`/api/submissions/${id}/status`, { status: newStatus });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["submissions"] });
    }
  });

  return (
    <div className="space-y-6">
      <div className="rounded-2xl bg-white p-6 shadow-sm">
        <h2 className="mb-4 text-lg font-semibold text-slate-800">لیست فرم‌های دانشجویان</h2>
        <div className="grid grid-cols-1 gap-4 md:grid-cols-4">
          <div>
            <label className="mb-2 block text-sm text-slate-600">جستجو</label>
            <input
              type="text"
              value={search}
              onChange={(event) => setSearch(event.target.value)}
              className="w-full rounded border border-slate-200 px-3 py-2 focus:border-primary focus:outline-none"
              placeholder="نام، شماره دانشجویی یا استاد"
            />
          </div>
          <div>
            <label className="mb-2 block text-sm text-slate-600">از تاریخ</label>
            <input
              type="date"
              value={from}
              onChange={(event) => setFrom(event.target.value)}
              className="w-full rounded border border-slate-200 px-3 py-2 focus:border-primary focus:outline-none"
            />
          </div>
          <div>
            <label className="mb-2 block text-sm text-slate-600">تا تاریخ</label>
            <input
              type="date"
              value={to}
              onChange={(event) => setTo(event.target.value)}
              className="w-full rounded border border-slate-200 px-3 py-2 focus:border-primary focus:outline-none"
            />
          </div>
          <div>
            <label className="mb-2 block text-sm text-slate-600">وضعیت</label>
            <select
              value={status}
              onChange={(event) => setStatus(event.target.value)}
              className="w-full rounded border border-slate-200 px-3 py-2 focus:border-primary focus:outline-none"
            >
              {statusOptions.map((option) => (
                <option key={option.value} value={option.value}>
                  {option.label}
                </option>
              ))}
            </select>
          </div>
        </div>
      </div>
      <div className="overflow-hidden rounded-2xl bg-white shadow-sm">
        <table className="min-w-full divide-y divide-slate-200 text-sm">
          <thead className="bg-slate-50 text-slate-600">
            <tr>
              <th className="px-4 py-3 text-right font-medium">دانشجو</th>
              <th className="px-4 py-3 text-right font-medium">شماره دانشجویی</th>
              <th className="px-4 py-3 text-right font-medium">استاد پیشنهادی</th>
              <th className="px-4 py-3 text-right font-medium">عنوان پژوهش</th>
              <th className="px-4 py-3 text-right font-medium">زمان ارسال</th>
              <th className="px-4 py-3 text-right font-medium">وضعیت</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-200">
            {isLoading && (
              <tr>
                <td colSpan={6} className="px-4 py-6 text-center text-slate-500">
                  در حال بارگذاری...
                </td>
              </tr>
            )}
            {isError && (
              <tr>
                <td colSpan={6} className="px-4 py-6 text-center text-red-500">
                  دریافت اطلاعات با خطا همراه شد
                </td>
              </tr>
            )}
            {!isLoading && !isError && data && data.length === 0 && (
              <tr>
                <td colSpan={6} className="px-4 py-6 text-center text-slate-500">
                  داده‌ای ثبت نشده است
                </td>
              </tr>
            )}
            {!isLoading && !isError &&
              data?.map((item) => (
                <tr key={item.submissionId}>
                  <td className="px-4 py-3">{item.fullName}</td>
                  <td className="px-4 py-3">{item.studentNumber}</td>
                  <td className="px-4 py-3">{item.teacherName}</td>
                  <td className="px-4 py-3">{item.topicTitle}</td>
                  <td className="px-4 py-3">{new Date(item.submittedAt).toLocaleString("fa-IR")}</td>
                  <td className="px-4 py-3">
                    <select
                      className="rounded border border-slate-300 px-2 py-1 text-sm focus:border-primary focus:outline-none"
                      value={item.status}
                      onChange={(event) =>
                        updateStatusMutation.mutate({
                          id: item.submissionId,
                          newStatus: event.target.value as SubmissionStatus
                        })
                      }
                    >
                      {Object.entries(statusLabels).map(([key, label]) => (
                        <option key={key} value={key}>
                          {label}
                        </option>
                      ))}
                    </select>
                  </td>
                </tr>
              ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default SubmissionsPage;
