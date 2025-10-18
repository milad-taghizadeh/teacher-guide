export type SubmissionStatus = "Pending" | "UnderReview" | "Approved" | "Rejected";

export const statusLabelMap: Record<SubmissionStatus, string> = {
  Pending: "در انتظار بررسی",
  UnderReview: "در حال بررسی",
  Approved: "تایید شده",
  Rejected: "رد شده"
};
