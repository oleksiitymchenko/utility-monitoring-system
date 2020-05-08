import { AppPage } from './app.po';
import { browser, logging } from 'protractor';

describe('workspace-project App', () => {
  let page: AppPage;

  beforeEach(() => {
    page = new AppPage();
  });

  it('should display application title: MonitoringSpa', () => {
    page.navigateTo();
      expect(page.getAppTitle()).toEqual('MonitoringSpa');
  });
});
