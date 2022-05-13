import { createSlice } from "@reduxjs/toolkit";

export const userSLice = createSlice({
  name: "user",
  initialState: [],
  reducers: {
    getUsers: (state, action) => {
      return { ...state, ...action.payload };
    },
  },
});
export const { getUsers } = userSLice.actions;
