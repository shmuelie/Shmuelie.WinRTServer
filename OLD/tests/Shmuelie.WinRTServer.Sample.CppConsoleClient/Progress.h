// Based on progress and spinner from WinGet

#pragma once

#include <atomic>
#include <future>
#include <string>
#include <vector>

// Displays an indefinite spinner.
struct IndefiniteSpinner
{
    IndefiniteSpinner() {}

    void ShowSpinner();

    void StopSpinner();

private:
    std::atomic<bool> m_canceled = false;
    std::atomic<bool> m_spinnerRunning = false;
    std::future<void> m_spinnerJob;

    void ShowSpinnerInternal();
};

// Displays progress
class ProgressBar
{
public:
    ProgressBar() {}

    void ShowProgress(uint64_t current, uint64_t maximum);

    void EndProgress(bool hideProgressWhenDone);

private:
    std::atomic<bool> m_isVisible = false;
    uint64_t m_lastCurrent = 0;

    void ClearLine();

    void ShowProgressNoVT(uint64_t current, uint64_t maximum);
};