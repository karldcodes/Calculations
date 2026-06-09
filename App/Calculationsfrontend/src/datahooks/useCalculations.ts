import useSWR from 'swr'
import type { Calculation } from '../types/calculation';
import { get } from '../clients/get';


// wrapper to abstract and seperate api call logic
export function useCalculations() {
    const { data, error, isLoading } = useSWR<Calculation[]>("/calculations", get);

    return {
        calculations: data ?? [],
        isLoading,
        error
    };
}