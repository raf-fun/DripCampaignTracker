import axios from 'axios'

const api = axios.create({
  baseURL: 'http://localhost:5066/api'
})

export const getCampaigns = () => api.get('/Campaign')
export const getCampaign = (id) => api.get(`/Campaign/${id}`)
export const createCampaign = (data) => api.post('/Campaign', data)
export const getMessages = (conversationId) => api.get(`/Conversation/${conversationId}/messages`)
export const sendMessage = (conversationId, data) => api.post(`/Conversation/${conversationId}/messages`, data)
export const getLeads = () => api.get('/Lead')