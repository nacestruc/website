<template>
  <b-table
    responsive
    bordered
    outlined
    striped
    hover
    sort-by="date"
    :sort-desc="true"
    sticky-header="600px"
    :items="csvdata"
    :fields="fields"
  >
    <template v-slot:head()="scope">
      <div class="text-nowrap">{{ scope.label }}</div>
    </template>
    <template v-slot:cell(date)="data">
      <div class="text-nowrap">{{ data.item.dateLocal }}</div>
    </template>
  </b-table>
</template>

<script>
export default {
  props: ["csvdata"],
  data() {
    return {
      fields: [
        {
          key: "date",
          headerTitle: "Datum",
          label: "Datum",
          sortable: true,
          stickyColumn: true,
          variant: 'grey'
        },
        {
          key: "age.female.todate",
          label: "Okuženih žensk"
        },
        {
          key: "age.male.todate",
          label: "Okuženih moških"
        }
      ]
    };
  }
};
</script>

<style lang="scss">
.b-table-sticky-column {
  width: 120px;
}
</style>