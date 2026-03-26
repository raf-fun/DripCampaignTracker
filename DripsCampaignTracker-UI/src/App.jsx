import { BrowserRouter, Routes, Route } from 'react-router-dom'
import CampaignList from './pages/CampaignList'
import CampaignDetail from './pages/CampaignDetail'
import ConversationView from './pages/ConversationView'
import CreateCampaign from './pages/CreateCampaign'

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<CampaignList />} />
        <Route path="/campaigns/:id" element={<CampaignDetail />} />
        <Route path="/conversations/:id" element={<ConversationView />} />
        <Route path="/campaigns/create" element={<CreateCampaign />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App